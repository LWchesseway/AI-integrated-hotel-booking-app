using DoAn.HotelParking.Core.Application.Interfaces.Booking;
using DoAn.HotelParking.Core.Domain.Enums;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.Booking;

public class BookingRepository : GenericRepository<Domain.Entities.Booking.Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Domain.Entities.Booking.Booking>> GetBookingsByCustomerAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Room)
            .Include(b => b.Customer)
            .Include(b => b.Payment)
            .Where(b => b.CustomerId == customerId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Booking.Booking>> GetBookingsByRoomAsync(int roomId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Room)
            .Include(b => b.Customer)
            .Include(b => b.Payment)
            .Where(b => b.RoomId == roomId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Booking.Booking>> GetBookingsByStatusAsync(int status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Room)
            .Include(b => b.Customer)
            .Include(b => b.Payment)
            .Where(b => (int)b.Status == status)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Booking.Booking?> GetBookingDetailAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
            .Include(b => b.Customer)
            .Include(b => b.ApprovedByUser)
            .Include(b => b.CancelledByUser)
            .Include(b => b.Payment)
            .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Booking.Booking>> GetPendingBookingsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Room)
            .Include(b => b.Customer)
            .Include(b => b.Payment)
            .Where(b => b.Status == BookingStatus.Pending)
            .OrderBy(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Booking.Booking>> GetTodayBookingsAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await DbSet
            .Include(b => b.Room)
            .Include(b => b.Customer)
            .Include(b => b.Payment)
            .Where(b => b.CreatedAt.Date == today)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
