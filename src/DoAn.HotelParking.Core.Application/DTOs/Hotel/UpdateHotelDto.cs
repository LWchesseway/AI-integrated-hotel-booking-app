using System.ComponentModel.DataAnnotations;
using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Application.DTOs.Complex;

public class UpdateComplexDto
{
    [Required(ErrorMessage = "Tên khách sạn là bắt buộc")]
    public string Name { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Ward { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    public string Phone { get; set; } = string.Empty;

    public string? Description { get; set; }
    public HotelStatus Status { get; set; }
    public bool IsDeleted { get; set; }
}