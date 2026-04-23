using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Booking;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Booking;
using DoAn.HotelParking.Core.Application.Interfaces.OwnerSetting;
using DoAn.HotelParking.Core.Application.Interfaces.Payment;
using DoAn.HotelParking.Core.Application.Interfaces.Room;
using DoAn.HotelParking.Core.Application.Interfaces.TimeSlot;
using DoAn.HotelParking.Core.Domain.Enums;
using BookingEntity = DoAn.HotelParking.Core.Domain.Entities.Booking.Booking;
using PaymentEntity = DoAn.HotelParking.Core.Domain.Entities.Booking.Payment;

namespace DoAn.HotelParking.Core.Application.Services.Booking;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IOwnerSettingService _ownerSettingService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BookingService(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IPaymentRepository paymentRepository,
        ITimeSlotRepository timeSlotRepository,
        IOwnerSettingService ownerSettingService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _paymentRepository = paymentRepository;
        _timeSlotRepository = timeSlotRepository;
        _ownerSettingService = ownerSettingService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _bookingRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<BookingDto>>(items);
    }

    public async Task<(IEnumerable<BookingDto> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _bookingRepository.GetPagedAsync(pageIndex, pageSize, null, cancellationToken);
        return (_mapper.Map<IEnumerable<BookingDto>>(items), totalCount);
    }

    public async Task<BookingDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _bookingRepository.GetByIdAsync(id, cancellationToken);
        return item is null ? default : _mapper.Map<BookingDto>(item);
    }

    public async Task<BookingDto> CreateAsync(CreateBookingDto dto, CancellationToken cancellationToken = default)
    {
        var booking = await CreateBookingCoreAsync(
            dto.CustomerId,
            dto.RoomId,
            dto.CheckInDate,
            dto.CheckOutDate,
            dto.GuestCount,
            dto.PaidAmount,
            dto.Note,
            dto.PaymentMethod,
            dto.TransactionCode,
            dto.PaymentNote,
            dto.Status,
            cancellationToken);

        return _mapper.Map<BookingDto>(booking);
    }

    public async Task<BookingDto> CreateCustomerBookingAsync(int customerId, CustomerCreateBookingRequestDto dto, CancellationToken cancellationToken = default)
    {
        var booking = await CreateBookingCoreAsync(
            customerId,
            dto.RoomId,
            dto.CheckInDate,
            dto.CheckOutDate,
            dto.GuestCount,
            dto.PaidAmount,
            dto.Note,
            dto.PaymentMethod,
            dto.TransactionCode,
            dto.PaymentNote,
            (byte)BookingStatus.Pending,
            cancellationToken);

        return _mapper.Map<BookingDto>(booking);
    }

    public async Task<(IEnumerable<BookingDto> Items, int TotalCount)> GetMyBookingsAsync(
        int customerId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _bookingRepository.GetByCustomerIdPagedAsync(customerId, pageIndex, pageSize, cancellationToken);
        return (_mapper.Map<IEnumerable<BookingDto>>(items), totalCount);
    }

    public async Task<BookingDto?> CancelMyBookingAsync(int bookingId, int customerId, string? reason, CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking is null || booking.CustomerId != customerId)
        {
            return default;
        }

        if (booking.Status is BookingStatus.Cancelled or BookingStatus.Completed)
        {
            return default;
        }

        booking.Status = BookingStatus.Cancelled;
        booking.CancelledBy = customerId;
        booking.CancelReason = reason;
        booking.CancelledAt = DateTime.UtcNow;
        booking.UpdatedAt = DateTime.UtcNow;

        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<BookingDto>(booking);
    }

    public async Task<BookingDto?> UpdateAsync(int id, UpdateBookingDto dto, CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository.GetByIdAsync(id, cancellationToken);
        if (booking is null)
        {
            return default;
        }

        var checkInDate = dto.CheckInDate.Date;
        var checkOutDate = dto.CheckOutDate.Date;
        if (checkOutDate <= checkInDate)
        {
            throw new InvalidOperationException("Check-out date must be after check-in date.");
        }

        if (dto.GuestCount <= 0)
        {
            throw new InvalidOperationException("Guest count must be greater than zero.");
        }

        var room = await _roomRepository.GetByIdWithHotelAsync(dto.RoomId, cancellationToken)
            ?? throw new KeyNotFoundException("Room not found.");

        var nightCount = (checkOutDate - checkInDate).Days;

        var hasOverlap = await _bookingRepository.HasOverlappingBookingAsync(dto.RoomId, checkInDate, checkOutDate, id, cancellationToken);
        if (hasOverlap)
        {
            throw new InvalidOperationException("Room has already been booked for the selected dates.");
        }

        var unitPrice = await ResolveNightlyPriceAsync(dto.RoomId, checkInDate, checkOutDate, room.Price, cancellationToken);
        var totalAmount = unitPrice * nightCount;
        if (dto.PaidAmount > totalAmount)
        {
            throw new InvalidOperationException("Paid amount cannot exceed total amount.");
        }

        booking.RoomId = dto.RoomId;
        booking.CustomerId = dto.CustomerId;
        booking.CheckInDate = checkInDate;
        booking.CheckOutDate = checkOutDate;
        booking.NightCount = nightCount;
        booking.GuestCount = dto.GuestCount;
        booking.RoomUnitPrice = unitPrice;
        booking.TotalAmount = totalAmount;
        booking.PaidAmount = dto.PaidAmount;
        booking.Note = dto.Note;
        booking.Status = dto.PaidAmount >= totalAmount ? BookingStatus.Confirmed : (BookingStatus)dto.Status;
        booking.CancelledBy = dto.CancelledBy;
        booking.CancelReason = dto.CancelReason;
        booking.CancelledAt = dto.CancelledAt;
        booking.UpdatedAt = DateTime.UtcNow;

        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<BookingDto>(booking);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository.GetByIdAsync(id, cancellationToken);
        if (booking is null)
        {
            return false;
        }

        _bookingRepository.Remove(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task AdminForceCompleteBookingAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken)
            ?? throw new KeyNotFoundException("Booking not found.");

        if (booking.Status == BookingStatus.Cancelled)
        {
            throw new InvalidOperationException("Cancelled booking cannot be force-completed.");
        }

        booking.Status = BookingStatus.Completed;
        booking.UpdatedAt = DateTime.UtcNow;

        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<BookingEntity> CreateBookingCoreAsync(
        int customerId,
        int roomId,
        DateTime checkInDate,
        DateTime checkOutDate,
        int guestCount,
        decimal paidAmount,
        string? note,
        string? paymentMethod,
        string? transactionCode,
        string? paymentNote,
        byte statusHint,
        CancellationToken cancellationToken)
    {
        var checkIn = checkInDate.Date;
        var checkOut = checkOutDate.Date;

        if (checkOut <= checkIn)
        {
            throw new InvalidOperationException("Check-out date must be after check-in date.");
        }

        if (guestCount <= 0)
        {
            throw new InvalidOperationException("Guest count must be greater than zero.");
        }

        var room = await _roomRepository.GetByIdWithHotelAsync(roomId, cancellationToken)
            ?? throw new KeyNotFoundException("Room not found.");

        if (room.IsDeleted || room.Status != RoomStatus.Available)
        {
            throw new InvalidOperationException("Room is not available for booking.");
        }

        var nightCount = (checkOut - checkIn).Days;

        var hasOverlap = await _bookingRepository.HasOverlappingBookingAsync(roomId, checkIn, checkOut, null, cancellationToken);
        if (hasOverlap)
        {
            throw new InvalidOperationException("Room has already been booked for the selected dates.");
        }

        var unitPrice = await ResolveNightlyPriceAsync(roomId, checkIn, checkOut, room.Price, cancellationToken);
        var totalAmount = unitPrice * nightCount;
        if (paidAmount < 0)
        {
            throw new InvalidOperationException("Paid amount cannot be negative.");
        }

        if (paidAmount > totalAmount)
        {
            throw new InvalidOperationException("Paid amount cannot exceed total amount.");
        }

        if (paidAmount > 0)
        {
            var ownerHasValidBankInfo = await _ownerSettingService.ValidateBankInfoAsync(room.Hotel.OwnerId, cancellationToken);
            if (!ownerHasValidBankInfo)
            {
                throw new InvalidOperationException("Hotel owner has not completed bank information for receiving payments.");
            }
        }

        var desiredStatus = (BookingStatus)statusHint;
        var finalStatus = paidAmount >= totalAmount
            ? BookingStatus.Confirmed
            : desiredStatus is BookingStatus.Cancelled or BookingStatus.Completed
                ? desiredStatus
                : BookingStatus.Pending;

        var booking = new BookingEntity
        {
            RoomId = roomId,
            CustomerId = customerId,
            CheckInDate = checkIn,
            CheckOutDate = checkOut,
            NightCount = nightCount,
            GuestCount = guestCount,
            RoomUnitPrice = unitPrice,
            TotalAmount = totalAmount,
            PaidAmount = paidAmount,
            Note = note,
            Status = finalStatus,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _bookingRepository.AddAsync(booking, cancellationToken);

        if (paidAmount > 0)
        {
            var payment = new PaymentEntity
            {
                Booking = booking,
                Amount = paidAmount,
                Method = paymentMethod,
                Status = PaymentStatus.Completed,
                TransactionCode = transactionCode,
                Note = paymentNote,
                PaidAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return booking;
    }

    private async Task<decimal> ResolveNightlyPriceAsync(
        int roomId,
        DateTime checkInDate,
        DateTime checkOutDate,
        decimal fallbackRoomPrice,
        CancellationToken cancellationToken)
    {
        var slot = await _timeSlotRepository.GetActiveByRoomAndDateRangeAsync(roomId, checkInDate, checkOutDate, cancellationToken);
        var unitPrice = slot?.Price ?? fallbackRoomPrice;
        if (unitPrice < 0)
        {
            throw new InvalidOperationException("Room price cannot be negative.");
        }

        return unitPrice;
    }
}