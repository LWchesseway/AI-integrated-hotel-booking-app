using System.ComponentModel.DataAnnotations;
using DoAn.HotelParking.Core.Domain.Enums;
namespace DoAn.HotelParking.Core.Application.DTOs.User;

public class UpdateUserDto
{
    [Required(ErrorMessage = "Họ là bắt buộc")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Tên là bắt buộc")]
    public string FirstName { get; set; } = null!;

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string? Phone { get; set; }

    public string? AvatarUrl { get; set; }

    public UserStatus Status { get; set; }
}
