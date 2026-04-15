using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Notification;

public class UpdateNotificationDto
{
    [MaxLength(255)]
    public string? Title { get; set; }

    public string? Message { get; set; }

    public byte Type { get; set; }

    [MaxLength(100)]
    public string? RelatedTable { get; set; }

    public int? RelatedId { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }
}