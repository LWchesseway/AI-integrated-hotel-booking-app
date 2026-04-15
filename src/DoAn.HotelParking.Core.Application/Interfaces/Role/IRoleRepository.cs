using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Auth;

namespace DoAn.HotelParking.Core.Application.Interfaces.Role;

public interface IRoleRepository : IGenericRepository<DoAn.HotelParking.Core.Domain.Entities.Auth.Role>
{
    Task<DoAn.HotelParking.Core.Domain.Entities.Auth.Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
