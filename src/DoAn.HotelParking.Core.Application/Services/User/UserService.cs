using AutoMapper;
using BCrypt.Net;
using DoAn.HotelParking.Core.Application.DTOs.User;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.User;
using DoAn.HotelParking.Core.Application.Services.Base;
using DoAn.HotelParking.Core.Domain.Entities.Auth;
using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Application.Services.User;

public class UserService : CrudService<DoAn.HotelParking.Core.Domain.Entities.Auth.User, UserDto, CreateUserDto, UpdateUserDto>, IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(userRepository, unitOfWork, mapper)
    {
        _userRepository = userRepository;
    }

    public override async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var existing = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var entity = Mapper.Map<DoAn.HotelParking.Core.Domain.Entities.Auth.User>(dto);
        entity.Email = normalizedEmail;
        entity.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        entity.Status = UserStatus.Active;
        entity.IsDeleted = false;

        await _userRepository.AddAsync(entity, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        return Mapper.Map<UserDto>(entity);
    }
}