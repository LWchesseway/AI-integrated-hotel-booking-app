namespace DoAn.HotelParking.Core.Application.DTOs.Hotel;

public class FavoriteStatusDto
{
    public bool IsFavorite { get; set; }
}

public class ToggleFavoriteResponseDto
{
    public bool IsFavorite { get; set; }
    public string Message { get; set; } = null!;
}
