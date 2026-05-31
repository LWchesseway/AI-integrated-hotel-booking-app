using DoAn.HotelParking.Core.Application.DTOs.Auth;
using DoAn.HotelParking.Core.Application.Interfaces.Auth;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Role;
using DoAn.HotelParking.Core.Application.Interfaces.User;
using DoAn.HotelParking.Core.Application.Services.Auth;
using DoAn.HotelParking.Core.Domain.Entities.Auth;
using DoAn.HotelParking.Core.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;
using AuthUser = DoAn.HotelParking.Core.Domain.Entities.Auth.User;

namespace DoAn.HotelParking.UnitTests.Services.Auth;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_ShouldCreateUserAndReturnTokens_WhenEmailNew()
    {
        var userRepository = new Mock<IUserRepository>();
        var roleRepository = new Mock<IRoleRepository>();
        var userRoleRepository = new Mock<IUserRoleRepository>();
        var refreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var tokenService = new Mock<ITokenService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var request = new RegisterRequestDto
        {
            FirstName = "Linh",
            LastName = "Tran",
            Email = "  LINH@EXAMPLE.COM  ",
            Phone = "0123456789",
            Password = "Pass123!",
            Role = string.Empty
        };

        userRepository
            .Setup(repo => repo.GetByEmailAsync("linh@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((AuthUser?)null);

        var role = new Role { Id = 3, Name = "Customer" };
        roleRepository
            .Setup(repo => repo.GetByNameAsync("Customer", It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        AuthUser? addedUser = null;
        userRepository
            .Setup(repo => repo.AddAsync(It.IsAny<AuthUser>(), It.IsAny<CancellationToken>()))
            .Callback<AuthUser, CancellationToken>((user, _) => addedUser = user)
            .Returns(Task.CompletedTask);

        UserRole? addedUserRole = null;
        userRoleRepository
            .Setup(repo => repo.AddAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()))
            .Callback<UserRole, CancellationToken>((userRole, _) => addedUserRole = userRole)
            .Returns(Task.CompletedTask);

        RefreshToken? addedRefreshToken = null;
        refreshTokenRepository
            .Setup(repo => repo.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Callback<RefreshToken, CancellationToken>((refreshToken, _) => addedRefreshToken = refreshToken)
            .Returns(Task.CompletedTask);

        tokenService.Setup(service => service.GenerateRefreshToken()).Returns("refresh-1");
        tokenService
            .Setup(service => service.GenerateAccessToken(It.IsAny<AuthUser>(), It.IsAny<IEnumerable<string>>()))
            .Returns("access-1");
        var accessTokenExpiry = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        tokenService.Setup(service => service.GetAccessTokenExpiresAt()).Returns(accessTokenExpiry);

        unitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService(
            userRepository,
            roleRepository,
            userRoleRepository,
            refreshTokenRepository,
            tokenService,
            unitOfWork);

        var response = await service.RegisterAsync(request);

        addedUser.Should().NotBeNull();
        addedUser!.Email.Should().Be("linh@example.com");
        addedUser.Status.Should().Be(UserStatus.Active);

        addedUserRole.Should().NotBeNull();
        addedUserRole!.RoleId.Should().Be(role.Id);

        addedRefreshToken.Should().NotBeNull();
        addedRefreshToken!.Token.Should().Be("refresh-1");

        response.Email.Should().Be("linh@example.com");
        response.Roles.Should().ContainSingle("Customer");
        response.AccessToken.Should().Be("access-1");
        response.RefreshToken.Should().Be("refresh-1");
        response.AccessTokenExpiresAt.Should().Be(accessTokenExpiry);

        unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrow_WhenEmailExists()
    {
        var userRepository = new Mock<IUserRepository>();
        var service = CreateService(
            userRepository,
            new Mock<IRoleRepository>(),
            new Mock<IUserRoleRepository>(),
            new Mock<IRefreshTokenRepository>(),
            new Mock<ITokenService>(),
            new Mock<IUnitOfWork>());

        var request = new RegisterRequestDto
        {
            FirstName = "Linh",
            LastName = "Tran",
            Email = "linh@example.com",
            Phone = "0123456789",
            Password = "Pass123!",
            Role = "Customer"
        };

        userRepository
            .Setup(repo => repo.GetByEmailAsync("linh@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthUser());

        var action = () => service.RegisterAsync(request);

        await action
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Email already exists.");
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenPasswordInvalid()
    {
        var userRepository = new Mock<IUserRepository>();
        var service = CreateService(
            userRepository,
            new Mock<IRoleRepository>(),
            new Mock<IUserRoleRepository>(),
            new Mock<IRefreshTokenRepository>(),
            new Mock<ITokenService>(),
            new Mock<IUnitOfWork>());

        var user = new AuthUser
        {
            Id = 1,
            Email = "user@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("Correct123"),
            Status = UserStatus.Active,
            IsDeleted = false
        };

        userRepository
            .Setup(repo => repo.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var request = new LoginRequestDto
        {
            Email = "user@example.com",
            Password = "Wrong123"
        };

        var action = () => service.LoginAsync(request);

        await action
            .Should()
            .ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password.");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnTokens_WhenValidCredentials()
    {
        var userRepository = new Mock<IUserRepository>();
        var userRoleRepository = new Mock<IUserRoleRepository>();
        var refreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var tokenService = new Mock<ITokenService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var service = CreateService(
            userRepository,
            new Mock<IRoleRepository>(),
            userRoleRepository,
            refreshTokenRepository,
            tokenService,
            unitOfWork);

        var user = new AuthUser
        {
            Id = 7,
            Email = "user@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("Correct123"),
            Status = UserStatus.Active,
            IsDeleted = false
        };

        userRepository
            .Setup(repo => repo.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        userRoleRepository
            .Setup(repo => repo.GetRoleNamesByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "Customer" });

        RefreshToken? addedRefreshToken = null;
        refreshTokenRepository
            .Setup(repo => repo.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Callback<RefreshToken, CancellationToken>((refreshToken, _) => addedRefreshToken = refreshToken)
            .Returns(Task.CompletedTask);

        tokenService.Setup(service => service.GenerateRefreshToken()).Returns("refresh-1");
        tokenService
            .Setup(service => service.GenerateAccessToken(It.IsAny<AuthUser>(), It.IsAny<IEnumerable<string>>()))
            .Returns("access-1");
        var accessTokenExpiry = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        tokenService.Setup(service => service.GetAccessTokenExpiresAt()).Returns(accessTokenExpiry);

        unitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new LoginRequestDto
        {
            Email = "user@example.com",
            Password = "Correct123"
        };

        var response = await service.LoginAsync(request);

        response.AccessToken.Should().Be("access-1");
        response.RefreshToken.Should().Be("refresh-1");
        response.Roles.Should().ContainSingle("Customer");
        response.AccessTokenExpiresAt.Should().Be(accessTokenExpiry);
        addedRefreshToken.Should().NotBeNull();
        addedRefreshToken!.UserId.Should().Be(user.Id);

        unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldRotateToken_WhenValid()
    {
        var userRepository = new Mock<IUserRepository>();
        var userRoleRepository = new Mock<IUserRoleRepository>();
        var refreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var tokenService = new Mock<ITokenService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var service = CreateService(
            userRepository,
            new Mock<IRoleRepository>(),
            userRoleRepository,
            refreshTokenRepository,
            tokenService,
            unitOfWork);

        var storedToken = new RefreshToken
        {
            Id = 1,
            UserId = 10,
            Token = "old-refresh",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false
        };

        refreshTokenRepository
            .Setup(repo => repo.GetByTokenAsync("old-refresh", It.IsAny<CancellationToken>()))
            .ReturnsAsync(storedToken);

        var user = new AuthUser
        {
            Id = 10,
            Email = "user@example.com",
            Status = UserStatus.Active,
            IsDeleted = false
        };

        userRepository
            .Setup(repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        userRoleRepository
            .Setup(repo => repo.GetRoleNamesByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "Customer" });

        refreshTokenRepository
            .Setup(repo => repo.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        tokenService.Setup(service => service.GenerateRefreshToken()).Returns("new-refresh");
        tokenService
            .Setup(service => service.GenerateAccessToken(It.IsAny<AuthUser>(), It.IsAny<IEnumerable<string>>()))
            .Returns("access-1");
        var accessTokenExpiry = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        tokenService.Setup(service => service.GetAccessTokenExpiresAt()).Returns(accessTokenExpiry);

        unitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new RefreshTokenRequestDto { RefreshToken = "old-refresh" };

        var response = await service.RefreshTokenAsync(request);

        storedToken.IsRevoked.Should().BeTrue();
        refreshTokenRepository.Verify(repo => repo.Update(storedToken), Times.Once);
        refreshTokenRepository.Verify(repo => repo.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        response.RefreshToken.Should().Be("new-refresh");
        response.AccessToken.Should().Be("access-1");
        response.AccessTokenExpiresAt.Should().Be(accessTokenExpiry);
    }

    private static AuthService CreateService(
        Mock<IUserRepository> userRepository,
        Mock<IRoleRepository> roleRepository,
        Mock<IUserRoleRepository> userRoleRepository,
        Mock<IRefreshTokenRepository> refreshTokenRepository,
        Mock<ITokenService> tokenService,
        Mock<IUnitOfWork> unitOfWork)
    {
        return new AuthService(
            userRepository.Object,
            roleRepository.Object,
            userRoleRepository.Object,
            refreshTokenRepository.Object,
            tokenService.Object,
            unitOfWork.Object);
    }
}
