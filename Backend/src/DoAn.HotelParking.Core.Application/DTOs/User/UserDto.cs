namespace DoAn.HotelParking.Core.Application.DTOs.User;

public class UserDto
{
    public int Id { get; set; }
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public byte Status { get; set; }
    public DateTime CreatedAt { get; set; }
}