namespace DoAn.HotelParking.Infrastructure.Storage;

public class MinioSettings
{
    public string Endpoint { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = "hotel-images";
    public bool UseSsl { get; set; }
    public string? PublicBaseUrl { get; set; }
}
