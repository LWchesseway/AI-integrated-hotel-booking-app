using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Booking;
using DoAn.HotelParking.Core.Application.Interfaces.Rooms;
using DoAn.HotelParking.Core.Domain.Entities.Booking;
using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Application.Services.Booking;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BookingService(IBookingRepository bookingRepository, IRoomRepository roomRepository, IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<IEnumerable<BookingDto>>> GetAllBookingsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var bookings = await _bookingRepository.GetAllAsync(cancellationToken);
            var dtos = bookings.Select(MapBookingToDto).ToList();
            return new ApiResponseDto<IEnumerable<BookingDto>> { Success = true, Data = dtos };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<BookingDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiPagedResponseDto<BookingDto>> GetBookingsPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var (bookings, totalCount) = await _bookingRepository.GetPagedAsync(pageIndex, pageSize, cancellationToken: cancellationToken);
            var dtos = bookings.Select(MapBookingToDto).ToList();
            return new ApiPagedResponseDto<BookingDto>
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
            return new ApiPagedResponseDto<BookingDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<BookingDetailDto>> GetBookingByIdAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await _bookingRepository.GetBookingDetailAsync(bookingId, cancellationToken);
            if (booking == null)
                return new ApiResponseDto<BookingDetailDto> { Success = false, Message = "Đặt phòng không tìm thấy" };

            var dto = MapBookingToDetailDto(booking);
            return new ApiResponseDto<BookingDetailDto> { Success = true, Data = dto };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<BookingDetailDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<IEnumerable<BookingDto>>> GetMyBookingsAsync(int customerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var bookings = await _bookingRepository.GetBookingsByCustomerAsync(customerId, cancellationToken);
            var dtos = bookings.Select(MapBookingToDto).ToList();
            return new ApiResponseDto<IEnumerable<BookingDto>> { Success = true, Data = dtos };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<BookingDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<IEnumerable<BookingDto>>> GetPendingBookingsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var bookings = await _bookingRepository.GetPendingBookingsAsync(cancellationToken);
            var dtos = bookings.Select(MapBookingToDto).ToList();
            return new ApiResponseDto<IEnumerable<BookingDto>> { Success = true, Data = dtos };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<BookingDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<BookingDto>> CreateBookingAsync(CreateBookingDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Kiểm tra phòng tồn tại không
            var room = await _roomRepository.GetByIdAsync(dto.RoomId, cancellationToken);
            if (room == null)
                return new ApiResponseDto<BookingDto> { Success = false, Message = "Phòng không tìm thấy" };

            // Kiểm tra khách hàng tồn tại không
            var customer = await _context.Users.FindAsync([dto.CustomerId], cancellationToken: cancellationToken);
            if (customer == null)
                return new ApiResponseDto<BookingDto> { Success = false, Message = "Khách hàng không tìm thấy" };

            var booking = new Domain.Entities.Booking.Booking
            {
                RoomId = dto.RoomId,
                CustomerId = dto.CustomerId,
                TotalAmount = dto.TotalAmount,
                DepositAmount = dto.DepositAmount,
                Note = dto.Note,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _bookingRepository.AddAsync(booking, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            booking.Room = room;
            booking.Customer = customer;
            var responseDto = MapBookingToDto(booking);
            return new ApiResponseDto<BookingDto> { Success = true, Data = responseDto, Message = "Tạo đặt phòng thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<BookingDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<BookingDto>> ApproveBookingAsync(int bookingId, int approvedBy, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken);
            if (booking == null)
                return new ApiResponseDto<BookingDto> { Success = false, Message = "Đặt phòng không tìm thấy" };

            if (booking.Status != BookingStatus.Pending)
                return new ApiResponseDto<BookingDto> { Success = false, Message = "Chỉ có thể duyệt đặt phòng ở trạng thái chờ duyệt" };

            booking.Status = BookingStatus.Confirmed;
            booking.ApprovedBy = approvedBy;
            booking.ApprovedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            _bookingRepository.Update(booking);
            await _context.SaveChangesAsync(cancellationToken);

            var updatedBooking = await _bookingRepository.GetBookingDetailAsync(bookingId, cancellationToken);
            var responseDto = MapBookingToDto(updatedBooking!);
            return new ApiResponseDto<BookingDto> { Success = true, Data = responseDto, Message = "Duyệt đặt phòng thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<BookingDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<BookingDto>> CancelBookingAsync(int bookingId, int cancelledBy, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken);
            if (booking == null)
                return new ApiResponseDto<BookingDto> { Success = false, Message = "Đặt phòng không tìm thấy" };

            if (booking.Status == BookingStatus.Cancelled)
                return new ApiResponseDto<BookingDto> { Success = false, Message = "Đặt phòng này đã bị hủy rồi" };

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledBy = cancelledBy;
            booking.CancelledAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            _bookingRepository.Update(booking);
            await _context.SaveChangesAsync(cancellationToken);

            var updatedBooking = await _bookingRepository.GetBookingDetailAsync(bookingId, cancellationToken);
            var responseDto = MapBookingToDto(updatedBooking!);
            return new ApiResponseDto<BookingDto> { Success = true, Data = responseDto, Message = "Hủy đặt phòng thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<BookingDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<BookingDto>> UploadPaymentProofAsync(int bookingId, string proofUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken);
            if (booking == null)
                return new ApiResponseDto<BookingDto> { Success = false, Message = "Đặt phòng không tìm thấy" };

            booking.PaymentProofUrl = proofUrl;
            booking.UpdatedAt = DateTime.UtcNow;

            _bookingRepository.Update(booking);
            await _context.SaveChangesAsync(cancellationToken);

            var updatedBooking = await _bookingRepository.GetBookingDetailAsync(bookingId, cancellationToken);
            var responseDto = MapBookingToDto(updatedBooking!);
            return new ApiResponseDto<BookingDto> { Success = true, Data = responseDto, Message = "Tải lên chứng minh thanh toán thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<BookingDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<BookingDto>> ConfirmPaymentAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken);
            if (booking == null)
                return new ApiResponseDto<BookingDto> { Success = false, Message = "Đặt phòng không tìm thấy" };

            var payment = new Payment
            {
                BookingId = bookingId,
                Amount = booking.TotalAmount,
                Status = PaymentStatus.Completed,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Payments.AddAsync(payment, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var updatedBooking = await _bookingRepository.GetBookingDetailAsync(bookingId, cancellationToken);
            var responseDto = MapBookingToDto(updatedBooking!);
            return new ApiResponseDto<BookingDto> { Success = true, Data = responseDto, Message = "Xác nhận thanh toán thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<BookingDto> { Success = false, Message = ex.Message };
        }
    }

    private BookingDto MapBookingToDto(Domain.Entities.Booking.Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            RoomNumber = booking.Room?.RoomNumber,
            CustomerId = booking.CustomerId,
            CustomerName = $"{booking.Customer?.FirstName} {booking.Customer?.LastName}".Trim(),
            TotalAmount = booking.TotalAmount,
            DepositAmount = booking.DepositAmount,
            Note = booking.Note,
            Status = (int)booking.Status,
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt
        };
    }

    private BookingDetailDto MapBookingToDetailDto(Domain.Entities.Booking.Booking booking)
    {
        var dto = new BookingDetailDto
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            RoomNumber = booking.Room?.RoomNumber,
            CustomerId = booking.CustomerId,
            CustomerName = $"{booking.Customer?.FirstName} {booking.Customer?.LastName}".Trim(),
            CustomerPhone = booking.Customer?.Phone,
            CustomerEmail = booking.Customer?.Email,
            TotalAmount = booking.TotalAmount,
            DepositAmount = booking.DepositAmount,
            Note = booking.Note,
            Status = (int)booking.Status,
            PaymentProofUrl = booking.PaymentProofUrl,
            ApprovedBy = booking.ApprovedBy,
            ApprovedByName = booking.ApprovedByUser != null
                ? $"{booking.ApprovedByUser.FirstName} {booking.ApprovedByUser.LastName}".Trim()
                : null,
            ApprovedAt = booking.ApprovedAt,
            CancelledBy = booking.CancelledBy,
            CancelledByName = booking.CancelledByUser != null
                ? $"{booking.CancelledByUser.FirstName} {booking.CancelledByUser.LastName}".Trim()
                : null,
            CancelledAt = booking.CancelledAt,
            HotelName = booking.Room?.Hotel?.Name,
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt
        };

        if (booking.Payment != null)
        {
            dto.Payment = new PaymentDto
            {
                Amount = booking.Payment.Amount,
                Method = booking.Payment.Method,
                Status = (int)booking.Payment.Status,
                TransactionCode = booking.Payment.TransactionCode,
                CreatedAt = booking.Payment.CreatedAt
            };
        }

        return dto;
    }
}
