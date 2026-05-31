using DoAn.HotelParking.Core.Application.Interfaces.SystemConfig;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using SystemConfigEntity = DoAn.HotelParking.Core.Domain.Entities.System.SystemConfig;

namespace DoAn.HotelParking.Infrastructure.Repositories.SystemConfig;

public class SystemConfigRepository : GenericRepository<SystemConfigEntity>, ISystemConfigRepository
{
    public SystemConfigRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<SystemConfigEntity?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        var normalizedKey = key.Trim().ToLower();
        return DbSet.FirstOrDefaultAsync(
            x => x.ConfigKey.ToLower() == normalizedKey,
            cancellationToken);
    }
}
