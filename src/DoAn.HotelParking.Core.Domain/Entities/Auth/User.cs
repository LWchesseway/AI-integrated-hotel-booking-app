using DoAn.HotelParking.Core.Domain.Enums;
using HotelEntity = DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel;

namespace DoAn.HotelParking.Core.Domain.Entities.Auth;

public class User
{
    public int Id { get; set; }
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Password { get; set; }
    public string? AvatarUrl { get; set; }
    public UserStatus Status { get; set; }
    public bool IsDeleted { get; set; }
    public int? DeletedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public User? DeletedByUser { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    public ICollection<QrCode> QrCodes { get; set; } = new HashSet<QrCode>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
    public ICollection<HotelEntity> OwnedHotels { get; set; } = new HashSet<HotelEntity>();
}