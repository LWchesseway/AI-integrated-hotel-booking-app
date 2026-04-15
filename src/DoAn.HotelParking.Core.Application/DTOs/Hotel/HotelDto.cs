namespace DoAn.HotelParking.Core.Application.DTOs.Hotel;

public class HotelDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Street { get; set; }
    public string? Ward { get; set; }
    public string? Province { get; set; }
    public string? Phone { get; set; }
    public string? Description { get; set; }
    public byte Status { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
}