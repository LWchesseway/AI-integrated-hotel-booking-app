using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Auth;

namespace DoAn.HotelParking.Core.Application.Interfaces.User;

public interface IUserRoleRepository : IGenericRepository<DoAn.HotelParking.Core.Domain.Entities.Auth.UserRole>
{
    Task<List<string>> GetRoleNamesByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
