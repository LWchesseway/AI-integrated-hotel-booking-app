namespace DoAn.HotelParking.Core.Application.DTOs.Notification;

public class NotificationDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? SenderId { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public byte Type { get; set; }
    public string? RelatedTable { get; set; }
    public int? RelatedId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
}