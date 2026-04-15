namespace DoAn.HotelParking.Core.Application.Interfaces.Base;

public interface ICrudService<TDto, TCreateDto, TUpdateDto>
{
    Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<TDto> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
    Task<TDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TDto> CreateAsync(TCreateDto dto, CancellationToken cancellationToken = default);
    Task<TDto?> UpdateAsync(int id, TUpdateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}