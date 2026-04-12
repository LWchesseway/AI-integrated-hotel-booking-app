namespace DoAn.HotelParking.Core.Application.DTOs.User;

using DoAn.HotelParking.Core.Domain.Enums;
public class UserProfileDto
{
    public int Id { get; set; }
    public string LastName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public List<string> RoleNames { get; set; } = new();
    public string? AvatarUrl { get; set; }
    public UserStatus Status { get; set; }
    public bool EmailVerified { get; set; }
}
