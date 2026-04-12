using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.User;

public class UpdateUserRoleDto
{
    [Required(ErrorMessage = "RoleId là bắt buộc")]
    public int RoleId { get; set; }
}
