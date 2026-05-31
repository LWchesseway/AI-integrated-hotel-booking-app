using DoAn.HotelParking.Core.Application.Interfaces.User;
using DoAn.HotelParking.Core.Domain.Entities.Auth;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.User;

public class UserRepository : GenericRepository<DoAn.HotelParking.Core.Domain.Entities.Auth.User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<DoAn.HotelParking.Core.Domain.Entities.Auth.User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }
}