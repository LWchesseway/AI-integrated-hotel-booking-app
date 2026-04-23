using DoAn.HotelParking.Core.Application.DTOs.OwnerSetting;

namespace DoAn.HotelParking.Core.Application.Interfaces.OwnerSetting;

public interface IOwnerSettingService
{
    Task<OwnerSettingDto?> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default);
    Task<OwnerSettingResponseDto> GetSettingsWithDefaultsAsync(int ownerId, CancellationToken cancellationToken = default);
    Task UpdateSettingsAsync(int ownerId, UpdateOwnerSettingDto dto, CancellationToken cancellationToken = default);
    Task UpdateBankInfoAsync(int ownerId, UpdateBankInfoDto dto, CancellationToken cancellationToken = default);
    Task<bool> ValidateBankInfoAsync(int ownerId, CancellationToken cancellationToken = default);
}
