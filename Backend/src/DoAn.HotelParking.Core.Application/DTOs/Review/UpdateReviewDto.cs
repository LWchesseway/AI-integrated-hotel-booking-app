using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Review;

public class UpdateReviewDto
{
    [Range(1, 5)]
    public byte Rating { get; set; }

    [MaxLength(500)]
    public string? Comment { get; set; }
}