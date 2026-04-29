using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.User;

public class UpdateFcmTokenDto
{
    [Required]
    public string Token { get; set; } = string.Empty;
}
