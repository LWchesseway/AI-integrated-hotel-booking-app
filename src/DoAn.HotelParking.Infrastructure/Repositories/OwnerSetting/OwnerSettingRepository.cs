using DoAn.HotelParking.Core.Application.Interfaces.OwnerSetting;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using OwnerSettingEntity = DoAn.HotelParking.Core.Domain.Entities.OwnerSetting.OwnerSetting;

namespace DoAn.HotelParking.Infrastructure.Repositories.OwnerSetting;

public class OwnerSettingRepository : GenericRepository<OwnerSettingEntity>, IOwnerSettingRepository
{
    public OwnerSettingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<OwnerSettingEntity?> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(x => x.OwnerId == ownerId, cancellationToken);
    }
}
