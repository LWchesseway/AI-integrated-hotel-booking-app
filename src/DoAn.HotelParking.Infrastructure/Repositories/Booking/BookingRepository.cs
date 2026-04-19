using DoAn.HotelParking.Core.Application.Interfaces.Booking;
using DoAn.HotelParking.Core.Domain.Enums;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.Booking;

public class BookingRepository : GenericRepository<DoAn.HotelParking.Core.Domain.Entities.Booking.Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<bool> HasOverlappingBookingAsync(
        int roomId,
        DateTime checkInDate,
        DateTime checkOutDate,
        int? excludeBookingId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Bookings
            .AsNoTracking()
            .Where(b =>
                b.RoomId == roomId
                && b.Status != BookingStatus.Cancelled
                && checkInDate < b.CheckOutDate
                && checkOutDate > b.CheckInDate);

        if (excludeBookingId.HasValue)
        {
            query = query.Where(b => b.Id != excludeBookingId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task<(IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Booking.Booking> Items, int TotalCount)> GetByCustomerIdPagedAsync(
        int customerId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Bookings
            .AsNoTracking()
            .Where(b => b.CustomerId == customerId)
            .OrderByDescending(b => b.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}