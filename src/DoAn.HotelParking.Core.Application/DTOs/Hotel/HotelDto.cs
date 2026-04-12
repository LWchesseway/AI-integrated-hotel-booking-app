using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Application.DTOs.Complex;

public class HotelDto
{
    public int Id { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Ward { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Description { get; set; }
    public HotelStatus Status { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public int RoomsCount { get; set; }
}