using System.Linq.Expressions;
using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.User;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.User;
using DoAn.HotelParking.Core.Domain.Entities.Auth;

namespace DoAn.HotelParking.Core.Application.Services.User;

public class PermissionManagementService : IPermissionManagementService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PermissionManagementService(
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<PermissionDto> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        string? keyword = null,
        string? module = null,
        CancellationToken cancellationToken = default)
    {
        pageIndex = pageIndex <= 0 ? 1 : pageIndex;
        pageSize = pageSize <= 0 ? 20 : pageSize;

        var normalizedKeyword = string.IsNullOrWhiteSpace(keyword)
            ? null
            : keyword.Trim().ToLowerInvariant();

        var normalizedModule = string.IsNullOrWhiteSpace(module)
            ? null
            : module.Trim().ToLowerInvariant();

        Expression<Func<Permission, bool>>? filter = null;

        if (normalizedKeyword is not null || normalizedModule is not null)
        {
            filter = permission =>
                (normalizedKeyword == null
                 || (permission.PermissionKey != null && permission.PermissionKey.ToLower().Contains(normalizedKeyword))
                 || (permission.Description != null && permission.Description.ToLower().Contains(normalizedKeyword)))
                && (normalizedModule == null
                    || (permission.Module != null && permission.Module.ToLower() == normalizedModule));
        }

        var (items, totalCount) = await _permissionRepository.GetPagedAsync(pageIndex, pageSize, filter, cancellationToken);
        return (_mapper.Map<IEnumerable<PermissionDto>>(items), totalCount);
    }

    public async Task<IEnumerable<PermissionDto>> GetByModuleAsync(string module, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(module))
        {
            return Enumerable.Empty<PermissionDto>();
        }

        var items = await _permissionRepository.GetByModuleAsync(module, cancellationToken);
        return _mapper.Map<IEnumerable<PermissionDto>>(items);
    }

    public async Task<IEnumerable<PermissionModuleDto>> GetGroupedByModuleAsync(CancellationToken cancellationToken = default)
    {
        var groupedItems = await _permissionRepository.GetAllGroupedByModuleAsync(cancellationToken);

        return groupedItems
            .OrderBy(group => group.Key)
            .Select(group => new PermissionModuleDto
            {
                Module = group.Key,
                Permissions = _mapper.Map<IEnumerable<PermissionDto>>(group.OrderBy(permission => permission.PermissionKey))
            })
            .ToList();
    }

    public async Task<PermissionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var permission = await _permissionRepository.GetByIdAsync(id, cancellationToken);
        return permission is null ? null : _mapper.Map<PermissionDto>(permission);
    }

    public async Task<PermissionDto> CreateAsync(CreatePermissionDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedPermissionKey = NormalizePermissionKey(dto.PermissionKey);

        if (await _permissionRepository.PermissionKeyExistsAsync(normalizedPermissionKey, null, cancellationToken))
        {
            throw new InvalidOperationException($"Permission key '{normalizedPermissionKey}' already exists.");
        }

        var entity = _mapper.Map<Permission>(dto);
        entity.PermissionKey = normalizedPermissionKey;
        entity.Module = NormalizeText(dto.Module);
        entity.Description = NormalizeText(dto.Description);

        await _permissionRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PermissionDto>(entity);
    }

    public async Task<PermissionDto?> UpdateAsync(int id, UpdatePermissionDto dto, CancellationToken cancellationToken = default)
    {
        var permission = await _permissionRepository.GetByIdAsync(id, cancellationToken);
        if (permission is null)
        {
            return null;
        }

        var normalizedPermissionKey = NormalizePermissionKey(dto.PermissionKey);

        if (await _permissionRepository.PermissionKeyExistsAsync(normalizedPermissionKey, id, cancellationToken))
        {
            throw new InvalidOperationException($"Permission key '{normalizedPermissionKey}' already exists.");
        }

        permission.PermissionKey = normalizedPermissionKey;
        permission.Module = NormalizeText(dto.Module);
        permission.Description = NormalizeText(dto.Description);

        _permissionRepository.Update(permission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PermissionDto>(permission);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var permission = await _permissionRepository.GetByIdAsync(id, cancellationToken);
        if (permission is null)
        {
            return false;
        }

        _permissionRepository.Remove(permission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static string NormalizePermissionKey(string permissionKey)
    {
        if (string.IsNullOrWhiteSpace(permissionKey))
        {
            throw new InvalidOperationException("Permission key is required.");
        }

        return permissionKey.Trim().ToLowerInvariant();
    }

    private static string? NormalizeText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
