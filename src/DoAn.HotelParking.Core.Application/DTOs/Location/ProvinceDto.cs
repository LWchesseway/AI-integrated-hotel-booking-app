namespace DoAn.HotelParking.Core.Application.DTOs.Location;

public class ProvinceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public bool IsActive { get; set; }
}
