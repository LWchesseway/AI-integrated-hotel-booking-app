using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.OwnerSetting;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.OwnerSetting;
using DoAn.HotelParking.Core.Application.Interfaces.Storage;
using DoAn.HotelParking.Core.Application.Interfaces.SystemConfig;
using OwnerSettingEntity = DoAn.HotelParking.Core.Domain.Entities.OwnerSetting.OwnerSetting;

namespace DoAn.HotelParking.Core.Application.Services.OwnerSetting;

public class OwnerSettingService : IOwnerSettingService
{
    private const string DefaultDepositRateKey = "DEFAULT_DEPOSIT_RATE";
    private const string MinBookingNoticeHoursKey = "MIN_BOOKING_NOTICE_HOURS";
    private const string EnableReviewSystemKey = "ENABLE_REVIEW_SYSTEM";

    private readonly IOwnerSettingRepository _ownerSettingRepository;
    private readonly ISystemConfigService _systemConfigService;
    private readonly IObjectStorageService _objectStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OwnerSettingService(
        IOwnerSettingRepository ownerSettingRepository,
        ISystemConfigService systemConfigService,
        IObjectStorageService objectStorageService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _ownerSettingRepository = ownerSettingRepository;
        _systemConfigService = systemConfigService;
        _objectStorageService = objectStorageService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OwnerSettingDto?> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default)
    {
        var entity = await _ownerSettingRepository.GetByOwnerIdAsync(ownerId, cancellationToken);
        return entity is null ? default : _mapper.Map<OwnerSettingDto>(entity);
    }

    public async Task<OwnerSettingResponseDto> GetSettingsWithDefaultsAsync(int ownerId, CancellationToken cancellationToken = default)
    {
        var ownerSetting = await _ownerSettingRepository.GetByOwnerIdAsync(ownerId, cancellationToken);

        var defaultDepositRate = await _systemConfigService.GetConfigValueAsync<decimal?>(DefaultDepositRateKey, cancellationToken) ?? 0.30m;
        var defaultMinBookingNotice = await _systemConfigService.GetConfigValueAsync<int?>(MinBookingNoticeHoursKey, cancellationToken) ?? 2;
        var defaultAllowReview = await _systemConfigService.GetConfigValueAsync<bool?>(EnableReviewSystemKey, cancellationToken) ?? true;

        return new OwnerSettingResponseDto
        {
            Data = ownerSetting is null ? default : _mapper.Map<OwnerSettingDto>(ownerSetting),
            SystemDefaults = new SystemDefaultsDto
            {
                DepositRate = defaultDepositRate,
                MinBookingNotice = defaultMinBookingNotice,
                AllowReview = defaultAllowReview
            }
        };
    }

    public async Task UpdateSettingsAsync(int ownerId, UpdateOwnerSettingDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.DepositRate is < 0 or > 1)
        {
            throw new InvalidOperationException("DepositRate must be between 0 and 1.");
        }

        if (dto.MinBookingNotice < 0)
        {
            throw new InvalidOperationException("MinBookingNotice must be greater than or equal to 0.");
        }

        var entity = await EnsureOwnerSettingAsync(ownerId, cancellationToken);

        _mapper.Map(dto, entity);
        entity.UpdatedAt = DateTime.UtcNow;

        _ownerSettingRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateBankInfoAsync(int ownerId, UpdateBankInfoDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.BankName)
            || string.IsNullOrWhiteSpace(dto.BankAccountNumber)
            || string.IsNullOrWhiteSpace(dto.BankAccountName))
        {
            throw new InvalidOperationException("BankName, BankAccountNumber and BankAccountName are required.");
        }

        var entity = await EnsureOwnerSettingAsync(ownerId, cancellationToken);

        entity.BankName = dto.BankName.Trim();
        entity.BankAccountNumber = dto.BankAccountNumber.Trim();
        entity.BankAccountName = dto.BankAccountName.Trim();

        if (dto.QrCodeImage is not null && dto.QrCodeImage.Length > 0)
        {
            var objectKey = BuildObjectKey(ownerId, dto.QrCodeImage.FileName);
            await using var stream = dto.QrCodeImage.OpenReadStream();
            var uploadedUrl = await _objectStorageService.UploadAsync(
                stream,
                dto.QrCodeImage.Length,
                objectKey,
                dto.QrCodeImage.ContentType,
                cancellationToken);

            entity.BankQrCodeUrl = uploadedUrl;
        }

        entity.UpdatedAt = DateTime.UtcNow;

        _ownerSettingRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ValidateBankInfoAsync(int ownerId, CancellationToken cancellationToken = default)
    {
        var entity = await _ownerSettingRepository.GetByOwnerIdAsync(ownerId, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(entity.BankName)
            && !string.IsNullOrWhiteSpace(entity.BankAccountNumber)
            && !string.IsNullOrWhiteSpace(entity.BankAccountName)
            && !string.IsNullOrWhiteSpace(entity.BankQrCodeUrl);
    }

    private async Task<OwnerSettingEntity> EnsureOwnerSettingAsync(int ownerId, CancellationToken cancellationToken)
    {
        var entity = await _ownerSettingRepository.GetByOwnerIdAsync(ownerId, cancellationToken);
        if (entity is not null)
        {
            return entity;
        }

        entity = new OwnerSettingEntity
        {
            OwnerId = ownerId,
            AllowReview = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _ownerSettingRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    private static string BuildObjectKey(int ownerId, string fileName)
    {
        var safeFileName = string.Concat(Path.GetFileName(fileName)
            .Select(ch => char.IsLetterOrDigit(ch) || ch is '.' or '-' or '_' ? ch : '-'));

        return $"owner-settings/{ownerId}/{Guid.NewGuid():N}-{safeFileName}";
    }
}
