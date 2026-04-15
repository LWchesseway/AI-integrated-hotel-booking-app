using DoAn.HotelParking.Core.Application.Interfaces.Review;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;

namespace DoAn.HotelParking.Infrastructure.Repositories.Review;

public class ReviewRepository : GenericRepository<DoAn.HotelParking.Core.Domain.Entities.Review.Review>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context)
    {
    }
}