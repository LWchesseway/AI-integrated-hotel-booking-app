using DoAn.HotelParking.Core.Application.Interfaces.Auth;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.Auth;

public class RefreshTokenRepository : GenericRepository<DoAn.HotelParking.Core.Domain.Entities.Auth.RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<DoAn.HotelParking.Core.Domain.Entities.Auth.RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
    }
}