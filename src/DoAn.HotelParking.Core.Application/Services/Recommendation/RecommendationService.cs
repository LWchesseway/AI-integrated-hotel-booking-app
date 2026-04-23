using DoAn.HotelParking.Core.Application.DTOs.Recommendation;
using DoAn.HotelParking.Core.Application.Interfaces.Booking;
using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using DoAn.HotelParking.Core.Application.Interfaces.Recommendation;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Application.Services.Recommendation;

public class RecommendationService : IRecommendationService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IBookingRepository _bookingRepository;

    public RecommendationService(
        IHotelRepository hotelRepository,
        IBookingRepository bookingRepository)
    {
        _hotelRepository = hotelRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<RecommendationResponse> GetSimilarHotelsAsync(int hotelId, int topK = 10, CancellationToken cancellationToken = default)
    {
        var normalizedTopK = NormalizeTopK(topK);

        var currentHotel = await _hotelRepository.GetHotelWithDetailsForRecommendationAsync(hotelId, cancellationToken);
        if (currentHotel is null)
        {
            return new RecommendationResponse
            {
                RecommendationType = "item-to-item",
                Hotels = new List<HotelRecommendationDto>(),
                Message = "Hotel not found"
            };
        }

        var currentVector = VectorizeHotel(currentHotel);

        var candidates = (await _hotelRepository.GetAllActiveHotelsWithDetailsAsync(
                currentHotel.Ward.Province.Name,
                currentHotel.Ward.Name,
                cancellationToken))
            .Where(h => h.Id != hotelId)
            .ToList();

        if (!candidates.Any())
        {
            candidates = (await _hotelRepository.GetAllActiveHotelsWithDetailsAsync(null, null, cancellationToken))
                .Where(h => h.Id != hotelId)
                .ToList();
        }

        var random = new Random();

        var hotels = candidates
            .Select(hotel =>
            {
                var score = CosineSimilarity(currentVector, VectorizeHotel(hotel));
                return (hotel, score, randomTie: random.Next());
            })
            .OrderByDescending(x => x.score)
            .ThenBy(x => x.randomTie)
            .Take(normalizedTopK)
            .Select(x => MapToRecommendationDto(x.hotel, x.score))
            .ToList();

        return new RecommendationResponse
        {
            RecommendationType = "item-to-item",
            Hotels = hotels,
            Message = $"Found {hotels.Count} similar hotels"
        };
    }

    public async Task<RecommendationResponse> GetRecommendationsForNewUserAsync(
        string? province,
        string? ward,
        int topK = 10,
        CancellationToken cancellationToken = default)
    {
        var normalizedTopK = NormalizeTopK(topK);

        var hotels = (await _hotelRepository.GetAllActiveHotelsWithDetailsAsync(province, ward, cancellationToken)).ToList();
        if (!hotels.Any() && (!string.IsNullOrWhiteSpace(province) || !string.IsNullOrWhiteSpace(ward)))
        {
            hotels = (await _hotelRepository.GetAllActiveHotelsWithDetailsAsync(null, null, cancellationToken)).ToList();
        }

        if (!hotels.Any())
        {
            return new RecommendationResponse
            {
                RecommendationType = "location-popularity",
                Hotels = new List<HotelRecommendationDto>(),
                Message = "No suitable hotels found"
            };
        }

        var maxBookingCount = Math.Max(hotels.Max(GetBookingCount), 1);
        var random = new Random();

        var recommendations = hotels
            .Select(hotel =>
            {
                var bookingCount = GetBookingCount(hotel);
                var avgRating = GetAverageRating(hotel);
                var normalizedBooking = (double)bookingCount / maxBookingCount;
                var normalizedRating = avgRating / 5.0;
                var popularityScore = (0.6 * normalizedBooking) + (0.4 * normalizedRating);

                return (hotel, popularityScore, randomTie: random.Next());
            })
            .OrderByDescending(x => x.popularityScore)
            .ThenByDescending(x => x.randomTie)
            .Take(normalizedTopK)
            .Select(x => MapToRecommendationDto(x.hotel, x.popularityScore))
            .ToList();

        return new RecommendationResponse
        {
            RecommendationType = "location-popularity",
            Hotels = recommendations,
            Message = $"Recommended {recommendations.Count} popular hotels"
        };
    }

    public async Task<RecommendationResponse> GetPersonalizedRecommendationsAsync(
        int userId,
        int topK = 10,
        string? province = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedTopK = NormalizeTopK(topK);

        var userHistory = (await _bookingRepository.GetUserBookingHistoryAsync(userId, cancellationToken)).ToList();
        if (!userHistory.Any())
        {
            return await GetRecommendationsForNewUserAsync(province, null, normalizedTopK, cancellationToken);
        }

        var bookedHotelIds = userHistory
            .Select(b => b.Room.HotelId)
            .Distinct()
            .ToHashSet();

        var bookedHotels = userHistory
            .Select(b => b.Room.Hotel)
            .GroupBy(h => h.Id)
            .Select(g => g.First())
            .ToList();

        var userVector = CreateUserVector(bookedHotels);

        var candidates = (await _hotelRepository.GetAllActiveHotelsWithDetailsAsync(province, null, cancellationToken))
            .Where(h => !bookedHotelIds.Contains(h.Id))
            .ToList();

        if (!candidates.Any())
        {
            return new RecommendationResponse
            {
                RecommendationType = "content-based-user",
                Hotels = new List<HotelRecommendationDto>(),
                Message = "No new hotels to recommend"
            };
        }

        var random = new Random();

        var recommendations = candidates
            .Select(hotel =>
            {
                var score = CosineSimilarity(userVector, VectorizeHotel(hotel));
                return (hotel, score, randomTie: random.Next());
            })
            .OrderByDescending(x => x.score)
            .ThenBy(x => x.randomTie)
            .Take(normalizedTopK)
            .Select(x => MapToRecommendationDto(x.hotel, x.score))
            .ToList();

        return new RecommendationResponse
        {
            RecommendationType = "content-based-user",
            Hotels = recommendations,
            Message = $"Recommended {recommendations.Count} hotels for you"
        };
    }

    private static int NormalizeTopK(int topK)
    {
        return Math.Clamp(topK, 1, 50);
    }

    private static double[] CreateUserVector(List<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel> bookedHotels)
    {
        if (!bookedHotels.Any())
        {
            return new double[6];
        }

        var vectors = bookedHotels.Select(VectorizeHotel).ToList();
        var dimension = vectors[0].Length;
        var result = new double[dimension];

        for (var i = 0; i < dimension; i++)
        {
            result[i] = vectors.Average(v => v[i]);
        }

        return result;
    }

    private static double[] VectorizeHotel(DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel hotel)
    {
        var roomCount = hotel.Rooms.Count;
        var averageRoomPrice = roomCount > 0 ? (double)hotel.Rooms.Average(r => r.Price) : 0;
        var averageRating = GetAverageRating(hotel);
        var bookingCount = GetBookingCount(hotel);
        var availableRoomCount = hotel.Rooms.Count(r => r.Status == RoomStatus.Available && !r.IsDeleted);
        var activeRoomRate = roomCount > 0 ? (double)availableRoomCount / roomCount : 0;

        return new[]
        {
            Math.Min(roomCount / 50.0, 1.0),
            Math.Min(averageRoomPrice / 5000000.0, 1.0),
            averageRating / 5.0,
            Math.Min(bookingCount / 500.0, 1.0),
            hotel.HotelImages.Any() ? 1.0 : 0.0,
            activeRoomRate
        };
    }

    private static double CosineSimilarity(double[] vectorA, double[] vectorB)
    {
        if (vectorA.Length != vectorB.Length)
        {
            return 0;
        }

        var dotProduct = 0.0;
        var magnitudeA = 0.0;
        var magnitudeB = 0.0;

        for (var i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            magnitudeA += vectorA[i] * vectorA[i];
            magnitudeB += vectorB[i] * vectorB[i];
        }

        if (magnitudeA == 0 || magnitudeB == 0)
        {
            return 0;
        }

        return dotProduct / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
    }

    private static int GetBookingCount(DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel hotel)
    {
        return hotel.Rooms
            .SelectMany(r => r.Bookings)
            .Count(b => b.Status is BookingStatus.Confirmed or BookingStatus.Completed);
    }

    private static double GetAverageRating(DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel hotel)
    {
        var ratings = hotel.Rooms
            .SelectMany(r => r.Reviews)
            .Select(r => (double)r.Rating)
            .ToList();

        return ratings.Count == 0 ? 0 : ratings.Average();
    }

    private static HotelRecommendationDto MapToRecommendationDto(DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel hotel, double score)
    {
        var allRooms = hotel.Rooms.Where(r => !r.IsDeleted).ToList();
        var averageRoomPrice = allRooms.Count > 0 ? allRooms.Average(r => r.Price) : 0;
        var imageUrl = hotel.HotelImages
            .OrderByDescending(i => i.IsPrimary)
            .ThenBy(i => i.SortOrder)
            .Select(i => i.ImageUrl)
            .FirstOrDefault();

        return new HotelRecommendationDto
        {
            Id = hotel.Id,
            Name = hotel.Name ?? string.Empty,
            OwnerId = hotel.OwnerId,
            Province = hotel.Ward.Province.Name,
            Ward = hotel.Ward.Name,
            AvgRoomPrice = averageRoomPrice,
            RoomCount = allRooms.Count,
            AverageRating = Math.Round(GetAverageRating(hotel), 2),
            BookingCount = GetBookingCount(hotel),
            SimilarityScore = Math.Round(score, 3),
            ImageUrl = imageUrl,
            IsActive = !hotel.IsDeleted && hotel.Status == HotelStatus.Active
        };
    }
}
