using DoAn.HotelParking.Core.Application.DTOs.SystemConfig;

namespace DoAn.HotelParking.Core.Application.Interfaces.SystemConfig;

public interface ISystemConfigService
{
    Task<IEnumerable<SystemConfigDto>> GetAllConfigsAsync(CancellationToken cancellationToken = default);
    Task<SystemConfigDto?> GetConfigByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<T?> GetConfigValueAsync<T>(string key, CancellationToken cancellationToken = default);
    Task UpdateConfigAsync(string key, UpdateSystemConfigDto dto, CancellationToken cancellationToken = default);
}
