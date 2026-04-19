namespace DoAn.HotelParking.Core.Application.DTOs.Hotel;

public class HotelImageDto
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public string? ImageUrl { get; set; }
    public string? ObjectKey { get; set; }
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}
