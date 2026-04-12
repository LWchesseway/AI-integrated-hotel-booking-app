using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Complex;

public class CreateComplexDto
{
    [Required(ErrorMessage = "Mã nhân viên là bắt buộc")]
    public int StaffId { get; set; }

    [Required(ErrorMessage = "Tên khu phức hợp là bắt buộc")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
    public string Street { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phường/Xã là bắt buộc")]
    public string Ward { get; set; } = string.Empty;

    [Required(ErrorMessage = "Quận/Huyện là bắt buộc")]
    public string Province { get; set; } = string.Empty;
    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    public string? Phone { get; set; }
    public string? Description { get; set; }
}
