using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Booking;

public class UploadPaymentProofDto
{
    [Required(ErrorMessage = "File chứng minh thanh toán là bắt buộc")]
    public IFormFile PaymentProofImage { get; set; }
    [MaxLength(255, ErrorMessage = "Ghi chú không được vượt quá 255 ký tự")]
    public string? PaymentNote { get; set; }
}