using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Rooms;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Application.Services.Rooms;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RoomService(IRoomRepository roomRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _roomRepository = roomRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponseDto<IEnumerable<RoomDto>>> GetAllRoomsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var rooms = await _roomRepository.GetAllAsync(r => !r.IsDeleted, cancellationToken);
            var dtos = rooms.Select(MapRoomToDto).ToList();
            return new ApiResponseDto<IEnumerable<RoomDto>> { Success = true, Data = dtos };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<RoomDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiPagedResponseDto<RoomDto>> GetRoomsPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var (rooms, totalCount) = await _roomRepository.GetPagedAsync(pageIndex, pageSize, r => !r.IsDeleted, cancellationToken);
            var dtos = rooms.Select(MapRoomToDto).ToList();
            return new ApiPagedResponseDto<RoomDto>
            {
                Success = true,
                Data = dtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            return new ApiPagedResponseDto<RoomDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<RoomDetailDto>> GetRoomByIdAsync(int roomId, CancellationToken cancellationToken = default)
    {
        try
        {
            var room = await _roomRepository.GetRoomDetailAsync(roomId, cancellationToken);
            if (room == null)
                return new ApiResponseDto<RoomDetailDto> { Success = false, Message = "Phòng không tìm thấy" };

            var averageRating = room.Reviews.Any() ? room.Reviews.Average(r => r.Rating) : 0;
            var dto = new RoomDetailDto
            {
                Id = room.Id,
                HotelId = room.HotelId,
                HotelName = room.Hotel.Name,
                HotelStreet = room.Hotel.Street,
                HotelPhone = room.Hotel.Phone,
                RoomTypeId = room.RoomTypeId,
                RoomTypeName = room.RoomType.Name,
                RoomNumber = room.RoomNumber,
                Capacity = room.Capacity,
                Status = (int)room.Status,
                CreatedAt = room.CreatedAt,
                BookingCount = room.Bookings.Count,
                AverageRating = averageRating,
                RoomImages = room.RoomImages.Select(ri => new RoomImageDto
                {
                    Id = ri.Id,
                    ImageUrl = ri.ImageUrl
                }).ToList()
            };

            return new ApiResponseDto<RoomDetailDto> { Success = true, Data = dto };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<RoomDetailDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<IEnumerable<RoomDto>>> GetRoomsByHotelAsync(int hotelId, CancellationToken cancellationToken = default)
    {
        try
        {
            var rooms = await _roomRepository.GetRoomsByHotelAsync(hotelId, cancellationToken);
            var dtos = rooms.Select(MapRoomToDto).ToList();
            return new ApiResponseDto<IEnumerable<RoomDto>> { Success = true, Data = dtos };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<RoomDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<IEnumerable<RoomDto>>> GetAvailableRoomsAsync(int hotelId, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken = default)
    {
        try
        {
            var rooms = await _roomRepository.GetAvailableRoomsAsync(hotelId, checkIn, checkOut, cancellationToken);
            var dtos = rooms.Select(MapRoomToDto).ToList();
            return new ApiResponseDto<IEnumerable<RoomDto>> { Success = true, Data = dtos };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<RoomDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<RoomDto>> CreateRoomAsync(CreateRoomDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Kiểm tra khách sạn tồn tại không
            var hotel = await _context.Hotels.FindAsync([dto.HotelId], cancellationToken: cancellationToken);
            if (hotel == null)
                return new ApiResponseDto<RoomDto> { Success = false, Message = "Khách sạn không tìm thấy" };

            // Kiểm tra loại phòng tồn tại không
            var roomType = await _context.RoomTypes.FindAsync([dto.RoomTypeId], cancellationToken: cancellationToken);
            if (roomType == null)
                return new ApiResponseDto<RoomDto> { Success = false, Message = "Loại phòng không tìm thấy" };

            // Kiểm tra số phòng đã tồn tại chưa
            var existingRoom = await _roomRepository.GetByRoomNumberAsync(dto.HotelId, dto.RoomNumber, cancellationToken);
            if (existingRoom != null)
                return new ApiResponseDto<RoomDto> { Success = false, Message = "Số phòng này đã tồn tại" };

            var room = new Room
            {
                HotelId = dto.HotelId,
                RoomTypeId = dto.RoomTypeId,
                RoomNumber = dto.RoomNumber,
                Capacity = dto.Capacity,
                Status = RoomStatus.Available,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _roomRepository.AddAsync(room, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            room.Hotel = hotel;
            room.RoomType = roomType;
            var responseDto = MapRoomToDto(room);
            return new ApiResponseDto<RoomDto> { Success = true, Data = responseDto, Message = "Tạo phòng thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<RoomDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<RoomDto>> UpdateRoomAsync(int roomId, UpdateRoomDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var room = await _roomRepository.GetByIdAsync(roomId, cancellationToken);
            if (room == null || room.IsDeleted)
                return new ApiResponseDto<RoomDto> { Success = false, Message = "Phòng không tìm thấy" };

            if (dto.RoomTypeId.HasValue)
            {
                var roomType = await _context.RoomTypes.FindAsync([dto.RoomTypeId.Value], cancellationToken: cancellationToken);
                if (roomType == null)
                    return new ApiResponseDto<RoomDto> { Success = false, Message = "Loại phòng không tìm thấy" };
                room.RoomTypeId = dto.RoomTypeId.Value;
            }

            room.RoomNumber = dto.RoomNumber ?? room.RoomNumber;
            room.Capacity = dto.Capacity ?? room.Capacity;

            _roomRepository.Update(room);
            await _context.SaveChangesAsync(cancellationToken);

            var updatedRoom = await _roomRepository.GetRoomDetailAsync(roomId, cancellationToken);
            var responseDto = MapRoomToDto(updatedRoom!);
            return new ApiResponseDto<RoomDto> { Success = true, Data = responseDto, Message = "Cập nhật phòng thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<RoomDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<bool>> DeleteRoomAsync(int roomId, CancellationToken cancellationToken = default)
    {
        try
        {
            var room = await _roomRepository.GetByIdAsync(roomId, cancellationToken);
            if (room == null || room.IsDeleted)
                return new ApiResponseDto<bool> { Success = false, Message = "Phòng không tìm thấy" };

            room.IsDeleted = true;
            _roomRepository.Update(room);
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponseDto<bool> { Success = true, Data = true, Message = "Xóa phòng thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<bool> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<RoomDto>> ChangeRoomStatusAsync(int roomId, int status, CancellationToken cancellationToken = default)
    {
        try
        {
            var room = await _roomRepository.GetByIdAsync(roomId, cancellationToken);
            if (room == null || room.IsDeleted)
                return new ApiResponseDto<RoomDto> { Success = false, Message = "Phòng không tìm thấy" };

            room.Status = (RoomStatus)status;
            _roomRepository.Update(room);
            await _context.SaveChangesAsync(cancellationToken);

            var updatedRoom = await _roomRepository.GetRoomDetailAsync(roomId, cancellationToken);
            var responseDto = MapRoomToDto(updatedRoom!);
            return new ApiResponseDto<RoomDto> { Success = true, Data = responseDto, Message = "Thay đổi trạng thái thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<RoomDto> { Success = false, Message = ex.Message };
        }
    }

    private RoomDto MapRoomToDto(Room room)
    {
        return new RoomDto
        {
            Id = room.Id,
            HotelId = room.HotelId,
            HotelName = room.Hotel?.Name,
            RoomTypeId = room.RoomTypeId,
            RoomTypeName = room.RoomType?.Name,
            RoomNumber = room.RoomNumber,
            Capacity = room.Capacity,
            Status = (int)room.Status,
            CreatedAt = room.CreatedAt
        };
    }
}
