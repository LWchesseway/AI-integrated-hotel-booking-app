using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Auth;

namespace DoAn.HotelParking.Core.Application.Interfaces.Auth;

public interface IFcmTokenRepository : IGenericRepository<FcmToken>
{
    Task<FcmToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FcmToken>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
