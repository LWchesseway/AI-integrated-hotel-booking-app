namespace DoAn.HotelParking.Core.Domain.Entities.Auth;

public class QrCode
{
    public int UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}