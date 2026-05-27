using DoAn.HotelParking.Core.Application.DTOs.Location;

namespace DoAn.HotelParking.Core.Application.Interfaces.Location;

public interface IProvinceService
{
	Task<IEnumerable<ProvinceDto>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<(IEnumerable<ProvinceDto> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
	Task<ProvinceDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<ProvinceDto> CreateAsync(CreateProvinceDto dto, CancellationToken cancellationToken = default);
	Task<ProvinceDto?> UpdateAsync(int id, UpdateProvinceDto dto, CancellationToken cancellationToken = default);
	Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
