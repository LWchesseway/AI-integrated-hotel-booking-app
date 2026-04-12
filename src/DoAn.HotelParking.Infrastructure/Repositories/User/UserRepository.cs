using DoAn.HotelParking.Core.Application.Interfaces.User;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.User;

public class UserRepository : GenericRepository<Domain.Entities.Auth.User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Domain.Entities.Auth.User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);
    }

    public async Task<Domain.Entities.Auth.User?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Phone == phone && !u.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Auth.User>> GetByStatusAsync(int status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(u => (int)u.Status == status && !u.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Auth.User>> GetByRoleAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(u => u.UserRoles)
            .Where(u => u.UserRoles.Any(ur => ur.RoleId == roleId) && !u.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Auth.User?> GetWithRolesAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Auth.User>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(u => (u.FirstName!.Contains(searchTerm)
                || u.LastName!.Contains(searchTerm)
                || u.Email!.Contains(searchTerm)
                || u.Phone!.Contains(searchTerm))
                && !u.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Auth.User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(u => !u.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}
