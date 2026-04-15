using DoAn.HotelParking.Core.Domain.Entities.Auth;

namespace DoAn.HotelParking.Core.Application.Interfaces.Auth;

public interface ITokenService
{
    string GenerateAccessToken(DoAn.HotelParking.Core.Domain.Entities.Auth.User user, IEnumerable<string> roles);
    string GenerateRefreshToken();
    DateTime GetAccessTokenExpiresAt();
}