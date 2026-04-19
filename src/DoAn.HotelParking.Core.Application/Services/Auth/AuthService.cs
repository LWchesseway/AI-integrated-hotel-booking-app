using BCrypt.Net;
using DoAn.HotelParking.Core.Application.DTOs.Auth;
using DoAn.HotelParking.Core.Application.Interfaces.Auth;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Role;
using DoAn.HotelParking.Core.Application.Interfaces.User;
using DoAn.HotelParking.Core.Domain.Entities.Auth;
using DoAn.HotelParking.Core.Domain.Enums;
using AuthUser = DoAn.HotelParking.Core.Domain.Entities.Auth.User;

namespace DoAn.HotelParking.Core.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existing = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var selectedRole = string.IsNullOrWhiteSpace(request.Role) ? "Customer" : request.Role.Trim();
        var role = await _roleRepository.GetByNameAsync(selectedRole, cancellationToken)
                   ?? await _roleRepository.GetByNameAsync("Customer", cancellationToken)
                   ?? throw new InvalidOperationException("Default role 'Customer' was not found.");

        var user = new AuthUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = normalizedEmail,
            Phone = request.Phone,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Status = UserStatus.Active,
            IsDeleted = false
        };

        await _userRepository.AddAsync(user, cancellationToken);

        await _userRoleRepository.AddAsync(new UserRole
        {
            User = user,
            RoleId = role.Id,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        var refreshToken = new RefreshToken
        {
            User = user,
            Token = _tokenService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var roles = new List<string> { role.Name ?? "Customer" };
        return BuildAuthResponse(user, roles, refreshToken.Token ?? string.Empty);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || string.IsNullOrWhiteSpace(user.Password) || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (user.Status != UserStatus.Active || user.IsDeleted)
        {
            throw new UnauthorizedAccessException("User is not active.");
        }

        var roles = await _userRoleRepository.GetRoleNamesByUserIdAsync(user.Id, cancellationToken);
        var refreshToken = CreateRefreshToken(user.Id);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return BuildAuthResponse(user, roles, refreshToken.Token ?? string.Empty);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (storedToken is null || storedToken.IsRevoked || storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        var user = await _userRepository.GetByIdAsync(storedToken.UserId, cancellationToken)
                   ?? throw new UnauthorizedAccessException("User not found for refresh token.");

        if (user.Status != UserStatus.Active || user.IsDeleted)
        {
            throw new UnauthorizedAccessException("User is not active.");
        }

        storedToken.IsRevoked = true;
        _refreshTokenRepository.Update(storedToken);

        var newRefreshToken = CreateRefreshToken(user.Id);
        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var roles = await _userRoleRepository.GetRoleNamesByUserIdAsync(user.Id, cancellationToken);
        return BuildAuthResponse(user, roles, newRefreshToken.Token ?? string.Empty);
    }

    private RefreshToken CreateRefreshToken(int userId)
    {
        return new RefreshToken
        {
            UserId = userId,
            Token = _tokenService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    private AuthResponseDto BuildAuthResponse(AuthUser user, IEnumerable<string> roles, string refreshToken)
    {
        var roleList = roles.ToList();
        return new AuthResponseDto
        {
            UserId = user.Id,
            FullName = $"{user.LastName} {user.FirstName}".Trim(),
            Email = user.Email ?? string.Empty,
            Roles = roleList,
            AccessToken = _tokenService.GenerateAccessToken(user, roleList),
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = _tokenService.GetAccessTokenExpiresAt()
        };
    }
}