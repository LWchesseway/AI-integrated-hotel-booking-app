using DoAn.HotelParking.Core.Application.Interfaces.Review;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.Review;

public class ReviewRepository : GenericRepository<Domain.Entities.Review.Review>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Domain.Entities.Review.Review>> GetReviewsByRoomAsync(int roomId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Room)
            .Include(r => r.Customer)
            .Include(r => r.Booking)
            .Where(r => r.RoomId == roomId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Review.Review>> GetReviewsByCustomerAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Room)
            .Include(r => r.Customer)
            .Include(r => r.Booking)
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Review.Review>> GetReviewsByBookingAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Room)
            .Include(r => r.Customer)
            .Include(r => r.Booking)
            .Where(r => r.BookingId == bookingId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Review.Review?> GetReviewDetailAsync(int reviewId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Room)
                .ThenInclude(room => room.Hotel)
            .Include(r => r.Customer)
            .Include(r => r.Booking)
            .FirstOrDefaultAsync(r => r.Id == reviewId, cancellationToken);
    }

    public async Task<bool> HasCustomerReviewedRoomAsync(int customerId, int roomId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(r => r.CustomerId == customerId && r.RoomId == roomId, cancellationToken);
    }

    public async Task<double> GetAverageRatingByRoomAsync(int roomId, CancellationToken cancellationToken = default)
    {
        var reviews = await DbSet
            .Where(r => r.RoomId == roomId)
            .ToListAsync(cancellationToken);

        return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
    }
}
