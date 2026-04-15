using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Auth;

namespace DoAn.HotelParking.Core.Application.Interfaces.User;

public interface IUserRepository : IGenericRepository<DoAn.HotelParking.Core.Domain.Entities.Auth.User>
{
    Task<DoAn.HotelParking.Core.Domain.Entities.Auth.User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
