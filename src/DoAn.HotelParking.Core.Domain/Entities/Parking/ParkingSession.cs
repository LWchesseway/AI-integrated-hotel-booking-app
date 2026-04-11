using DoAn.HotelParking.Core.Domain.Entities.Auth;
using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Domain.Entities.Parking;

public class ParkingSession
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int QrUserId { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? CheckInPlate { get; set; }
    public string? CheckOutPlate { get; set; }
    public ParkingSessionStatus Status { get; set; }
    public int? VerifiedBy { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public User QrUser { get; set; } = null!;
    public User? VerifiedByUser { get; set; }
    public ICollection<LicensePlateLog> LicensePlateLogs { get; set; } = new HashSet<LicensePlateLog>();
}