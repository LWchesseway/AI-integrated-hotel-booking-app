namespace DoAn.HotelParking.Core.Domain.Entities.Location;

public class Province
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public bool IsActive { get; set; }

    public ICollection<Ward> Wards { get; set; } = new HashSet<Ward>();
}
