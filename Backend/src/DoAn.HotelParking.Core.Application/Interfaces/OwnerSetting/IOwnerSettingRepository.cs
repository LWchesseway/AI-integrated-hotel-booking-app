using DoAn.HotelParking.Core.Application.Interfaces.Base;
using OwnerSettingEntity = DoAn.HotelParking.Core.Domain.Entities.OwnerSetting.OwnerSetting;

namespace DoAn.HotelParking.Core.Application.Interfaces.OwnerSetting;

public interface IOwnerSettingRepository : IGenericRepository<OwnerSettingEntity>
{
    Task<OwnerSettingEntity?> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default);
}
