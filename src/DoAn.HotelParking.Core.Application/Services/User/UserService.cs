using AutoMapper;
using BCrypt.Net;
using DoAn.HotelParking.Core.Application.DTOs.User;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.User;
using DoAn.HotelParking.Core.Domain.Enums;
using UserEntity = DoAn.HotelParking.Core.Domain.Entities.Auth.User;

namespace DoAn.HotelParking.Core.Application.Services.User;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _userRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<UserDto>>(entities);
    }


    public async Task<bool> UpdateFcmTokenAsync(int userId, string token)
{
        var user = await _userRepository.GetByIdAsync(userId);

    if (user == null) return false;

    user.FcmToken = token;
    user.FcmTokenUpdatedAt = DateTime.UtcNow;

    await _unitOfWork.SaveChangesAsync();
    return true;
}
    public async Task<(IEnumerable<UserDto> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _userRepository.GetPagedAsync(pageIndex, pageSize, null, cancellationToken);
        return (_mapper.Map<IEnumerable<UserDto>>(items), totalCount);
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _userRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? default : _mapper.Map<UserDto>(entity);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var existing = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var entity = _mapper.Map<UserEntity>(dto);
        entity.Email = normalizedEmail;
        entity.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        entity.Status = UserStatus.Active;
        entity.IsDeleted = false;

        await _userRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<UserDto>(entity);
    }

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return default;
        }

        _mapper.Map(dto, entity);
        entity.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<UserDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _userRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}