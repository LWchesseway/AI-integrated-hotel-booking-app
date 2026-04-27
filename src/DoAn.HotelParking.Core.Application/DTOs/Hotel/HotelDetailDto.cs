namespace DoAn.HotelParking.Core.Application.DTOs.Hotel;

public class HotelDetailDto
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public int WardId { get; set; }
    public string? Name { get; set; }
    public string? Street { get; set; }
    public string? Phone { get; set; }
    public string? Description { get; set; }
    public byte Status { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? WardName { get; set; }
    public string? ProvinceName { get; set; }
}
