using HotelEntity = DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel;

namespace DoAn.HotelParking.Core.Domain.Entities.Location;

public class Ward
{
    public int Id { get; set; }
    public int ProvinceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public bool IsActive { get; set; }

    public Province Province { get; set; } = null!;
    public ICollection<HotelEntity> Hotels { get; set; } = new HashSet<HotelEntity>();
}
