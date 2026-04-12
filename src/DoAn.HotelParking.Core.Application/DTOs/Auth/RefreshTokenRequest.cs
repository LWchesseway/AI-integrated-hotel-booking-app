using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Auth;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "Refresh token không được để trống")]
    public string RefreshToken { get; set; } = string.Empty;
}

