using DoAn.HotelParking.Core.Application.DTOs.User;
using System.IO;

namespace DoAn.HotelParking.Core.Application.Interfaces.User;

public interface IUserService
{
	Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);

	Task<bool> AddFcmTokenForUserAsync(int userId, string token);
	Task<(IEnumerable<UserDto> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
	Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
	Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken = default);
	Task<UserDto?> UpdateAvatarAsync(
		int userId,
		Stream stream,
		long fileSize,
		string fileName,
		string contentType,
		CancellationToken cancellationToken = default);
	Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}