using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Room;
using DoAn.HotelParking.Core.Application.Interfaces.Booking;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Room;
using DoAn.HotelParking.Core.Domain.Enums;
using RoomEntity = DoAn.HotelParking.Core.Domain.Entities.Hotel.Room;

namespace DoAn.HotelParking.Core.Application.Services.Room;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RoomService(
        IRoomRepository repository,
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _roomRepository = repository;
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RoomDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _roomRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<RoomDto>>(entities);
    }

    public async Task<(IEnumerable<RoomDto> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _roomRepository.GetPagedAsync(pageIndex, pageSize, null, cancellationToken);
        return (_mapper.Map<IEnumerable<RoomDto>>(items), totalCount);
    }

    public async Task<RoomDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _roomRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? default : _mapper.Map<RoomDto>(entity);
    }

    public async Task<IEnumerable<RoomDetailDto>> GetByRoomTypeIdAsync(int roomTypeId, CancellationToken cancellationToken = default)
    {
        var entities = await _roomRepository.GetByRoomTypeIdWithDetailsAsync(roomTypeId, cancellationToken);
        return _mapper.Map<IEnumerable<RoomDetailDto>>(entities);
    }

    public async Task<IEnumerable<RoomDetailDto>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default)
    {
        var entities = await _roomRepository.GetByHotelIdWithDetailsAsync(hotelId, cancellationToken);
        return _mapper.Map<IEnumerable<RoomDetailDto>>(entities);
    }

    public async Task<IEnumerable<RoomDetailDto>> GetAvailableByHotelIdAsync(
        int hotelId,
        DateTime checkInDate,
        DateTime checkOutDate,
        CancellationToken cancellationToken = default)
    {
        var checkIn = checkInDate.Date;
        var checkOut = checkOutDate.Date;
        if (checkOut <= checkIn)
        {
            throw new InvalidOperationException("Check-out date must be after check-in date.");
        }

        var rooms = (await _roomRepository.GetByHotelIdWithDetailsAsync(hotelId, cancellationToken))
            .Where(r => !r.IsDeleted && r.Status == RoomStatus.Available)
            .ToList();

        if (rooms.Count == 0)
        {
            return [];
        }

        var activeBookings = (await _bookingRepository.GetActiveBookingsByHotelAsync(
            hotelId,
            checkIn,
            checkOut,
            cancellationToken)).ToList();

        var bookedRoomIds = activeBookings
            .Where(b => checkIn < b.CheckOutDate && checkOut > b.CheckInDate)
            .Select(b => b.RoomId)
            .Distinct()
            .ToList();

        var availableRooms = rooms.Where(r => !bookedRoomIds.Contains(r.Id)).ToList();

        return _mapper.Map<IEnumerable<RoomDetailDto>>(availableRooms);
    }

    public async Task<IEnumerable<DateTime>> GetFullyBookedDatesByHotelIdAsync(
        int hotelId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var start = fromDate.Date;
        var end = toDate.Date;
        if (end < start)
        {
            throw new InvalidOperationException("toDate must be on or after fromDate.");
        }

        var activeRooms = (await _roomRepository.GetByHotelIdWithDetailsAsync(hotelId, cancellationToken))
            .Where(r => !r.IsDeleted && r.Status == RoomStatus.Available)
            .ToList();

        if (activeRooms.Count == 0)
        {
            return [];
        }

        var activeBookings = (await _bookingRepository.GetActiveBookingsByHotelAsync(
            hotelId,
            start,
            end.AddDays(1),
            cancellationToken)).ToList();
        var fullyBookedDates = new List<DateTime>();

        for (var day = start; day <= end; day = day.AddDays(1))
        {
            var bookedRoomsCount = activeBookings
                .Where(b => day < b.CheckOutDate && day.AddDays(1) > b.CheckInDate)
                .Select(b => b.RoomId)
                .Distinct()
                .Count();

            if (bookedRoomsCount >= activeRooms.Count)
            {
                fullyBookedDates.Add(day);
            }
        }

        return fullyBookedDates;
    }

    public async Task<IEnumerable<DateTime>> GetBookedDatesByRoomIdAsync(
        int roomId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var start = fromDate.Date;
        var end = toDate.Date;
        if (end < start)
        {
            throw new InvalidOperationException("toDate must be on or after fromDate.");
        }

        var room = await _roomRepository.GetByIdAsync(roomId, cancellationToken)
            ?? throw new KeyNotFoundException("Room not found.");

        var activeBookings = (await _bookingRepository.GetActiveBookingsByHotelAsync(
            room.HotelId,
            start,
            end.AddDays(1),
            cancellationToken))
            .Where(b => b.RoomId == roomId)
            .ToList();

        var bookedDates = new List<DateTime>();

        for (var day = start; day <= end; day = day.AddDays(1))
        {
            var hasBooking = activeBookings.Any(b => day < b.CheckOutDate && day.AddDays(1) > b.CheckInDate);
            if (hasBooking)
            {
                bookedDates.Add(day);
            }
        }

        return bookedDates;
    }
    public async Task<RoomDto> CreateAsync(CreateRoomDto dto, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<RoomEntity>(dto);
        await _roomRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<RoomDto>(entity);
    }

    public async Task<RoomDto?> UpdateAsync(int id, UpdateRoomDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _roomRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return default;
        }

        _mapper.Map(dto, entity);
        _roomRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<RoomDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _roomRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _roomRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
