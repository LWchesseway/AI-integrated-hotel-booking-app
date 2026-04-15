using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Review;

public class CreateReviewDto
{
    [Required]
    public int BookingId { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public int RoomId { get; set; }

    [Range(1, 5)]
    public byte Rating { get; set; }

    [MaxLength(500)]
    public string? Comment { get; set; }
}