using DoAn.HotelParking.Core.Application.Interfaces.Auth;
using DoAn.HotelParking.Core.Domain.Entities.Auth;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.Auth;

public class FcmTokenRepository : GenericRepository<FcmToken>, IFcmTokenRepository
{
    public FcmTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<FcmToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
    }

    public async Task<IReadOnlyList<FcmToken>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
    }
}
