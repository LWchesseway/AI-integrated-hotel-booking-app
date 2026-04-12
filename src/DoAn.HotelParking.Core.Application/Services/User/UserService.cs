using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.User;
using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Application.Services.User;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<IEnumerable<UserDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _userRepository.GetActiveUsersAsync(cancellationToken);
            var dtos = users.Select(MapToDto).ToList();
            return new ApiResponseDto<IEnumerable<UserDto>> { Success = true, Data = dtos };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<UserDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiPagedResponseDto<UserDto>> GetUsersPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var (users, totalCount) = await _userRepository.GetPagedAsync(pageIndex, pageSize, u => !u.IsDeleted, cancellationToken);
            var dtos = users.Select(MapToDto).ToList();
            return new ApiPagedResponseDto<UserDto>
            {
                Success = true,
                Data = dtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            return new ApiPagedResponseDto<UserDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<UserDetailDto>> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetWithRolesAsync(userId, cancellationToken);
            if (user == null)
                return new ApiResponseDto<UserDetailDto> { Success = false, Message = "Người dùng không tìm thấy" };

            var bookingCount = await _context.Bookings.CountAsync(b => b.CustomerId == userId, cancellationToken);
            var reviewCount = await _context.Reviews.CountAsync(r => r.CustomerId == userId, cancellationToken);
            var parkingCount = await _context.ParkingSessions.CountAsync(p => p.UserId == userId, cancellationToken);

            var dto = new UserDetailDto
            {
                Id = user.Id,
                LastName = user.LastName,
                FirstName = user.FirstName,
                Email = user.Email,
                Phone = user.Phone,
                AvatarUrl = user.AvatarUrl,
                Status = (int)user.Status,
                CreatedAt = user.CreatedAt,
                BookingCount = bookingCount,
                ReviewCount = reviewCount,
                ParkingSessionCount = parkingCount,
                Roles = user.UserRoles.Select(ur => new UserRoleDto
                {
                    Id = ur.Role.Id,
                    Name = ur.Role.Name,
                    Description = ur.Role.Description
                }).ToList()
            };

            return new ApiResponseDto<UserDetailDto> { Success = true, Data = dto };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<UserDetailDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<UserDto>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (user == null)
                return new ApiResponseDto<UserDto> { Success = false, Message = "Người dùng không tìm thấy" };

            var dto = MapToDto(user);
            return new ApiResponseDto<UserDto> { Success = true, Data = dto };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<UserDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<IEnumerable<UserDto>>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _userRepository.SearchByNameAsync(searchTerm, cancellationToken);
            var dtos = users.Select(MapToDto).ToList();
            return new ApiResponseDto<IEnumerable<UserDto>> { Success = true, Data = dtos };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<UserDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<UserDto>> UpdateUserAsync(int userId, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null || user.IsDeleted)
                return new ApiResponseDto<UserDto> { Success = false, Message = "Người dùng không tìm thấy" };

            user.LastName = dto.LastName ?? user.LastName;
            user.FirstName = dto.FirstName ?? user.FirstName;
            user.Phone = dto.Phone ?? user.Phone;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _context.SaveChangesAsync(cancellationToken);

            var responseDto = MapToDto(user);
            return new ApiResponseDto<UserDto> { Success = true, Data = responseDto, Message = "Cập nhật thông tin người dùng thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<UserDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<UserDto>> UpdateUserAvatarAsync(int userId, string avatarUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null || user.IsDeleted)
                return new ApiResponseDto<UserDto> { Success = false, Message = "Người dùng không tìm thấy" };

            user.AvatarUrl = avatarUrl;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _context.SaveChangesAsync(cancellationToken);

            var responseDto = MapToDto(user);
            return new ApiResponseDto<UserDto> { Success = true, Data = responseDto, Message = "Cập nhật avatar thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<UserDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<UserDto>> ChangeUserStatusAsync(int userId, int status, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null || user.IsDeleted)
                return new ApiResponseDto<UserDto> { Success = false, Message = "Người dùng không tìm thấy" };

            user.Status = (UserStatus)status;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _context.SaveChangesAsync(cancellationToken);

            var responseDto = MapToDto(user);
            return new ApiResponseDto<UserDto> { Success = true, Data = responseDto, Message = "Thay đổi trạng thái người dùng thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<UserDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<bool>> DeleteUserAsync(int userId, int deletedBy, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null || user.IsDeleted)
                return new ApiResponseDto<bool> { Success = false, Message = "Người dùng không tìm thấy" };

            user.IsDeleted = true;
            user.DeletedBy = deletedBy;
            user.DeletedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponseDto<bool> { Success = true, Data = true, Message = "Xóa người dùng thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<bool> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<UserDto>> RestoreUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null || !user.IsDeleted)
                return new ApiResponseDto<UserDto> { Success = false, Message = "Người dùng không tìm thấy" };

            user.IsDeleted = false;
            user.DeletedBy = null;
            user.DeletedAt = null;

            _userRepository.Update(user);
            await _context.SaveChangesAsync(cancellationToken);

            var responseDto = MapToDto(user);
            return new ApiResponseDto<UserDto> { Success = true, Data = responseDto, Message = "Khôi phục người dùng thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<UserDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<UserDetailDto>> AssignRoleAsync(int userId, int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetWithRolesAsync(userId, cancellationToken);
            if (user == null || user.IsDeleted)
                return new ApiResponseDto<UserDetailDto> { Success = false, Message = "Người dùng không tìm thấy" };

            var role = await _context.Roles.FindAsync([roleId], cancellationToken: cancellationToken);
            if (role == null)
                return new ApiResponseDto<UserDetailDto> { Success = false, Message = "Vai trò không tìm thấy" };

            // Kiểm tra người dùng đã có vai trò này chưa
            if (user.UserRoles.Any(ur => ur.RoleId == roleId))
                return new ApiResponseDto<UserDetailDto> { Success = false, Message = "Người dùng đã có vai trò này" };

            var userRole = new Domain.Entities.Auth.UserRole
            {
                UserId = userId,
                RoleId = roleId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.UserRoles.AddAsync(userRole, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var updatedUser = await _userRepository.GetWithRolesAsync(userId, cancellationToken);
            var dto = MapToDetailDto(updatedUser!);
            return new ApiResponseDto<UserDetailDto> { Success = true, Data = dto, Message = "Gán vai trò thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<UserDetailDto> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponseDto<UserDetailDto>> RemoveRoleAsync(int userId, int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetWithRolesAsync(userId, cancellationToken);
            if (user == null || user.IsDeleted)
                return new ApiResponseDto<UserDetailDto> { Success = false, Message = "Người dùng không tìm thấy" };

            var userRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == roleId);
            if (userRole == null)
                return new ApiResponseDto<UserDetailDto> { Success = false, Message = "Người dùng không có vai trò này" };

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync(cancellationToken);

            var updatedUser = await _userRepository.GetWithRolesAsync(userId, cancellationToken);
            var dto = MapToDetailDto(updatedUser!);
            return new ApiResponseDto<UserDetailDto> { Success = true, Data = dto, Message = "Hủy vai trò thành công" };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<UserDetailDto> { Success = false, Message = ex.Message };
        }
    }

    private UserDto MapToDto(Domain.Entities.Auth.User user)
    {
        return new UserDto
        {
            Id = user.Id,
            LastName = user.LastName,
            FirstName = user.FirstName,
            Email = user.Email,
            Phone = user.Phone,
            AvatarUrl = user.AvatarUrl,
            Status = (int)user.Status,
            CreatedAt = user.CreatedAt
        };
    }

    private UserDetailDto MapToDetailDto(Domain.Entities.Auth.User user)
    {
        var bookingCount = _context.Bookings.Count(b => b.CustomerId == user.Id);
        var reviewCount = _context.Reviews.Count(r => r.CustomerId == user.Id);
        var parkingCount = _context.ParkingSessions.Count(p => p.UserId == user.Id);

        return new UserDetailDto
        {
            Id = user.Id,
            LastName = user.LastName,
            FirstName = user.FirstName,
            Email = user.Email,
            Phone = user.Phone,
            AvatarUrl = user.AvatarUrl,
            Status = (int)user.Status,
            CreatedAt = user.CreatedAt,
            BookingCount = bookingCount,
            ReviewCount = reviewCount,
            ParkingSessionCount = parkingCount,
            Roles = user.UserRoles.Select(ur => new UserRoleDto
            {
                Id = ur.Role.Id,
                Name = ur.Role.Name,
                Description = ur.Role.Description
            }).ToList()
        };
    }
}
