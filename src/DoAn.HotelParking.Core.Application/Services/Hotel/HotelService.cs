using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Application.Services.Hotel;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HotelService(IHotelRepository hotelRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _hotelRepository = hotelRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponseDto<IEnumerable<HotelDto>>> GetAllHotelsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var hotels = await _hotelRepository.GetAllAsync(h => !h.IsDeleted, cancellationToken);
            var dtos = _mapper.Map<IEnumerable<HotelDto>>(hotels);
            return new ApiResponseDto<IEnumerable<HotelDto>> { Success = true, Data = dtos };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<HotelDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiPagedResponseDto<HotelDto>> GetHotelsPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var (hotels, totalCount) = await _hotelRepository.GetPagedAsync(pageIndex, pageSize, h => !h.IsDeleted, cancellationToken);
            var dtos = _mapper.Map<IEnumerable<HotelDto>>(hotels);
            return new ApiPagedResponseDto<HotelDto>
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
            return new ApiPagedResponseDto<HotelDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<HotelDetailDto>> GetHotelByIdAsync(int hotelId, CancellationToken cancellationToken = default)
    {
        try
        {
            var hotel = await _hotelRepository.GetWithRoomsAsync(hotelId, cancellationToken);
            if (hotel == null)
                return new ApiResponseDto<HotelDetailDto> { Success = false, Message = "Khách sạn không tìm thấy" };

            var dto = new HotelDetailDto
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Street = hotel.Street,
                Ward = hotel.Ward,
                Province = hotel.Province,
                Phone = hotel.Phone,
                Description = hotel.Description,
                Status = (int)hotel.Status,
                CreatedAt = hotel.CreatedAt,
                RoomCount = hotel.Rooms.Count,
                Rooms = hotel.Rooms.Select(r => new RoomBasicDto
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    Capacity = r.Capacity,
                    Status = (int)r.Status
                }).ToList()
            };

            return new ApiResponseDto<HotelDetailDto> { Success = true, Data = dto };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<HotelDetailDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<HotelDto>> GetHotelByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            var hotel = await _hotelRepository.GetByNameAsync(name, cancellationToken);
            if (hotel == null)
                return new ApiResponseDto<HotelDto> { Success = false, Message = "Khách sạn không tìm thấy" };

            var dto = _mapper.Map<HotelDto>(hotel);
            return new ApiResponseDto<HotelDto> { Success = true, Data = dto };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<HotelDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<IEnumerable<HotelDto>>> GetHotelsByProvinceAsync(string province, CancellationToken cancellationToken = default)
    {
        try
        {
            var hotels = await _hotelRepository.GetByProvinceAsync(province, cancellationToken);
            var dtos = _mapper.Map<IEnumerable<HotelDto>>(hotels);
            return new ApiResponseDto<IEnumerable<HotelDto>> { Success = true, Data = dtos };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<HotelDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<HotelDto>> CreateHotelAsync(CreateHotelDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Kiểm tra khách sạn đã tồn tại chưa
            var existingHotel = await _hotelRepository.GetByNameAsync(dto.Name, cancellationToken);
            if (existingHotel != null)
                return new ApiResponseDto<HotelDto> { Success = false, Message = "Khách sạn này đã tồn tại" };

            var hotel = new Domain.Entities.Hotel.Hotel
            {
                Name = dto.Name,
                Street = dto.Street,
                Ward = dto.Ward,
                Province = dto.Province,
                Phone = dto.Phone,
                Description = dto.Description,
                Status = HotelStatus.Active,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _hotelRepository.AddAsync(hotel, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var responseDto = _mapper.Map<HotelDto>(hotel);
            return new ApiResponseDto<HotelDto> { Success = true, Data = responseDto, Message = "Tạo khách sạn thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<HotelDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<HotelDto>> UpdateHotelAsync(int hotelId, UpdateHotelDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var hotel = await _hotelRepository.GetByIdAsync(hotelId, cancellationToken);
            if (hotel == null || hotel.IsDeleted)
                return new ApiResponseDto<HotelDto> { Success = false, Message = "Khách sạn không tìm thấy" };

            hotel.Name = dto.Name ?? hotel.Name;
            hotel.Street = dto.Street ?? hotel.Street;
            hotel.Ward = dto.Ward ?? hotel.Ward;
            hotel.Province = dto.Province ?? hotel.Province;
            hotel.Phone = dto.Phone ?? hotel.Phone;
            hotel.Description = dto.Description ?? hotel.Description;

            _hotelRepository.Update(hotel);
            await _context.SaveChangesAsync(cancellationToken);

            var responseDto = _mapper.Map<HotelDto>(hotel);
            return new ApiResponseDto<HotelDto> { Success = true, Data = responseDto, Message = "Cập nhật khách sạn thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<HotelDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<bool>> DeleteHotelAsync(int hotelId, CancellationToken cancellationToken = default)
    {
        try
        {
            var hotel = await _hotelRepository.GetByIdAsync(hotelId, cancellationToken);
            if (hotel == null || hotel.IsDeleted)
                return new ApiResponseDto<bool> { Success = false, Message = "Khách sạn không tìm thấy" };

            hotel.IsDeleted = true;
            _hotelRepository.Update(hotel);
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponseDto<bool> { Success = true, Data = true, Message = "Xóa khách sạn thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<bool> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<HotelDto>> ChangeHotelStatusAsync(int hotelId, int status, CancellationToken cancellationToken = default)
    {
        try
        {
            var hotel = await _hotelRepository.GetByIdAsync(hotelId, cancellationToken);
            if (hotel == null || hotel.IsDeleted)
                return new ApiResponseDto<HotelDto> { Success = false, Message = "Khách sạn không tìm thấy" };

            hotel.Status = (HotelStatus)status;
            _hotelRepository.Update(hotel);
            await _context.SaveChangesAsync(cancellationToken);

            var responseDto = _mapper.Map<HotelDto>(hotel);
            return new ApiResponseDto<HotelDto> { Success = true, Data = responseDto, Message = "Thay đổi trạng thái thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<HotelDto> { Success = false, Message = ex.Message };
        }
    }
}
