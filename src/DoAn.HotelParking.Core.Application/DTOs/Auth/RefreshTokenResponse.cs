using DoAn.HotelParking.Core.Application.DTOs.User;

namespace DoAn.HotelParking.Core.Application.DTOs.Auth;

public class RefreshTokenResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
}

