using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.SystemConfig;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.SystemConfig;

namespace DoAn.HotelParking.Core.Application.Services.SystemConfig;

public class SystemConfigService : ISystemConfigService
{
    private readonly ISystemConfigRepository _systemConfigRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SystemConfigService(
        ISystemConfigRepository systemConfigRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _systemConfigRepository = systemConfigRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SystemConfigDto>> GetAllConfigsAsync(CancellationToken cancellationToken = default)
    {
        var configs = await _systemConfigRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<SystemConfigDto>>(configs);
    }

    public async Task<SystemConfigDto?> GetConfigByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        var config = await _systemConfigRepository.GetByKeyAsync(key, cancellationToken);
        return config is null ? default : _mapper.Map<SystemConfigDto>(config);
    }

    public async Task<T?> GetConfigValueAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var config = await _systemConfigRepository.GetByKeyAsync(key, cancellationToken);
        if (config is null || string.IsNullOrWhiteSpace(config.ConfigValue))
        {
            return default;
        }

        try
        {
            var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            object converted;

            if (targetType == typeof(bool))
            {
                var raw = config.ConfigValue.Trim().ToLowerInvariant();
                converted = raw switch
                {
                    "1" or "true" or "yes" or "on" => true,
                    "0" or "false" or "no" or "off" => false,
                    _ => bool.Parse(config.ConfigValue)
                };
            }
            else if (targetType.IsEnum)
            {
                converted = Enum.Parse(targetType, config.ConfigValue, true);
            }
            else
            {
                converted = Convert.ChangeType(config.ConfigValue, targetType);
            }

            return (T?)converted;
        }
        catch
        {
            return default;
        }
    }

    public async Task UpdateConfigAsync(string key, UpdateSystemConfigDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.ConfigValue))
        {
            throw new InvalidOperationException("ConfigValue is required.");
        }

        var config = await _systemConfigRepository.GetByKeyAsync(key, cancellationToken)
            ?? throw new KeyNotFoundException($"SystemConfig with key '{key}' not found.");

        config.ConfigValue = dto.ConfigValue;
        config.UpdatedAt = DateTime.UtcNow;

        _systemConfigRepository.Update(config);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
