using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Booking;

namespace DoAn.HotelParking.Core.Application.Interfaces.Booking;

public interface IBookingRepository : IGenericRepository<DoAn.HotelParking.Core.Domain.Entities.Booking.Booking>
{
	Task<bool> HasOverlappingBookingAsync(
		int roomId,
		DateTime checkInDate,
		DateTime checkOutDate,
		int? excludeBookingId = null,
		CancellationToken cancellationToken = default);

	Task<(IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Booking.Booking> Items, int TotalCount)> GetByCustomerIdPagedAsync(
		int customerId,
		int pageIndex,
		int pageSize,
		CancellationToken cancellationToken = default);
}
