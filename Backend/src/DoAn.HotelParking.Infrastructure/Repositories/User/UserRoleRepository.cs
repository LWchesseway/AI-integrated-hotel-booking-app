using DoAn.HotelParking.Core.Application.Interfaces.User;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.User;

public class UserRoleRepository : GenericRepository<DoAn.HotelParking.Core.Domain.Entities.Auth.UserRole>, IUserRoleRepository
{
    public UserRoleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<string>> GetRoleNamesByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(
                Context.Roles,
                ur => ur.RoleId,
                r => r.Id,
                (_, role) => role.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}