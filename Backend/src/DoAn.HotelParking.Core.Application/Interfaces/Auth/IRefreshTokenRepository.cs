using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Auth;

namespace DoAn.HotelParking.Core.Application.Interfaces.Auth;

public interface IRefreshTokenRepository : IGenericRepository<DoAn.HotelParking.Core.Domain.Entities.Auth.RefreshToken>
{
    Task<DoAn.HotelParking.Core.Domain.Entities.Auth.RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
}
