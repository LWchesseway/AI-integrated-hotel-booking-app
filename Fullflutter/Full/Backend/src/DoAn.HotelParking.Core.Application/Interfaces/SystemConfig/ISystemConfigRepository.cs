using DoAn.HotelParking.Core.Application.Interfaces.Base;
using SystemConfigEntity = DoAn.HotelParking.Core.Domain.Entities.System.SystemConfig;

namespace DoAn.HotelParking.Core.Application.Interfaces.SystemConfig;

public interface ISystemConfigRepository : IGenericRepository<SystemConfigEntity>
{
    Task<SystemConfigEntity?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
}
