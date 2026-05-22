using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Notification;

public class CreateNotificationDto
{
    [Required]
    public int UserId { get; set; }

    public int? SenderId { get; set; }

    [MaxLength(255)]
    public string? Title { get; set; }

    public string? Message { get; set; }

    public byte Type { get; set; } = 0;

    [MaxLength(100)]
    public string? RelatedTable { get; set; }

    public int? RelatedId { get; set; }
}