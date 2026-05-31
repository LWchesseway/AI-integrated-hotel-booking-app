using DoAn.HotelParking.Core.Application.DTOs.Location;

namespace DoAn.HotelParking.Core.Application.Interfaces.Location;

public interface IWardService
{
	Task<IEnumerable<WardDto>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<(IEnumerable<WardDto> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
	Task<WardDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<WardDto> CreateAsync(CreateWardDto dto, CancellationToken cancellationToken = default);
	Task<WardDto?> UpdateAsync(int id, UpdateWardDto dto, CancellationToken cancellationToken = default);
	Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
