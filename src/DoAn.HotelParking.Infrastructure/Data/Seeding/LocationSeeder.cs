using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using DoAn.HotelParking.Core.Domain.Entities.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DoAn.HotelParking.Infrastructure.Data.Seeding;

public class LocationSeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly ILogger<LocationSeeder> _logger;

    public LocationSeeder(
        ApplicationDbContext context,
        IHttpClientFactory httpClientFactory,
        ILogger<LocationSeeder> logger)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public async Task SeedLocationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting location seeding process...");

            var hasProvinces = await _context.Provinces.AnyAsync(cancellationToken);
            var hasWards = await _context.Wards.AnyAsync(cancellationToken);

            if (hasProvinces && hasWards)
            {
                _logger.LogInformation("Location data already exists. Skipping seeding.");
                return;
            }

            _logger.LogInformation("Fetching provinces from API...");
            var provincesJson = await _httpClient.GetStringAsync("https://provinces.open-api.vn/api/v2/p/", cancellationToken);
            var provinceResponses = JsonSerializer.Deserialize<List<ProvinceApiResponse>>(provincesJson, JsonOptions);

            if (provinceResponses is null || provinceResponses.Count == 0)
            {
                _logger.LogWarning("No province data received from API.");
                return;
            }

            var existingProvinceCodes = await _context.Provinces
                .AsNoTracking()
                .Where(x => x.Code != null)
                .Select(x => x.Code!)
                .ToHashSetAsync(cancellationToken);

            var newProvinces = provinceResponses
                .Where(x => !existingProvinceCodes.Contains(x.Code.ToString()))
                .Select(x => new Province
                {
                    Name = x.Name,
                    Code = x.Code.ToString(),
                    IsActive = true
                })
                .ToList();

            if (newProvinces.Count > 0)
            {
                await _context.Provinces.AddRangeAsync(newProvinces, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var provinceIdByCode = await _context.Provinces
                .AsNoTracking()
                .Where(x => x.Code != null)
                .ToDictionaryAsync(x => x.Code!, x => x.Id, cancellationToken);

            var existingWardCodes = await _context.Wards
                .AsNoTracking()
                .Where(x => x.Code != null)
                .Select(x => x.Code!)
                .ToHashSetAsync(cancellationToken);

            var newWards = new List<Ward>();

            foreach (var provinceResponse in provinceResponses)
            {
                if (!provinceIdByCode.TryGetValue(provinceResponse.Code.ToString(), out var provinceId))
                {
                    _logger.LogWarning("Province code {ProvinceCode} not found in database, skipping wards.", provinceResponse.Code);
                    continue;
                }

                try
                {
                    var provinceDetailJson = await _httpClient.GetStringAsync(
                        $"https://provinces.open-api.vn/api/v2/p/{provinceResponse.Code}?depth=2",
                        cancellationToken);

                    var provinceDetail = JsonSerializer.Deserialize<ProvinceDetailApiResponse>(provinceDetailJson, JsonOptions);
                    if (provinceDetail?.Wards is null || provinceDetail.Wards.Count == 0)
                    {
                        continue;
                    }

                    foreach (var wardResponse in provinceDetail.Wards)
                    {
                        var wardCode = wardResponse.Code.ToString();
                        if (!existingWardCodes.Add(wardCode))
                        {
                            continue;
                        }

                        newWards.Add(new Ward
                        {
                            ProvinceId = provinceId,
                            Name = wardResponse.Name,
                            Code = wardCode,
                            IsActive = true
                        });
                    }

                    _logger.LogInformation(
                        "Collected wards for province {ProvinceName} (code {ProvinceCode}).",
                        provinceResponse.Name,
                        provinceResponse.Code);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error fetching wards for province {ProvinceName} (code {ProvinceCode}).",
                        provinceResponse.Name,
                        provinceResponse.Code);
                }
            }

            if (newWards.Count > 0)
            {
                await _context.Wards.AddRangeAsync(newWards, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation(
                "Location seeding completed. Added {ProvinceCount} provinces and {WardCount} wards.",
                newProvinces.Count,
                newWards.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during location seeding process.");
            throw;
        }
    }

    private sealed class ProvinceApiResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    private sealed class WardApiResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    private sealed class ProvinceDetailApiResponse
    {
        [JsonPropertyName("wards")]
        public List<WardApiResponse> Wards { get; set; } = [];
    }
}
