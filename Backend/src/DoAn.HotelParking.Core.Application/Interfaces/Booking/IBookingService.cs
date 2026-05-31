using DoAn.HotelParking.Core.Application.DTOs.Booking;

namespace DoAn.HotelParking.Core.Application.Interfaces.Booking;

public interface IBookingService
{
	Task<IEnumerable<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<(IEnumerable<BookingDto> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
	Task<BookingDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<BookingDto> CreateAsync(CreateBookingDto dto, CancellationToken cancellationToken = default);
	Task<BookingDto?> UpdateAsync(int id, UpdateBookingDto dto, CancellationToken cancellationToken = default);
	Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

	Task<BookingDto> CreateCustomerBookingAsync(int customerId, CustomerCreateBookingRequestDto dto, CancellationToken cancellationToken = default);
	Task<(IEnumerable<BookingDto> Items, int TotalCount)> GetMyBookingsAsync(int customerId, int pageIndex, int pageSize, CancellationToken cancellationToken = default);
	Task<BookingDto?> CancelMyBookingAsync(int bookingId, int customerId, string? reason, CancellationToken cancellationToken = default);
	Task AdminForceCompleteBookingAsync(int bookingId, CancellationToken cancellationToken = default);
}