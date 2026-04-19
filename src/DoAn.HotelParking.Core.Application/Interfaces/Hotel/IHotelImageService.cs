using DoAn.HotelParking.Core.Application.DTOs.Hotel;

namespace DoAn.HotelParking.Core.Application.Interfaces.Hotel;

public interface IHotelImageService
{
    Task<IEnumerable<HotelImageDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<HotelImageDto> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
    Task<HotelImageDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<HotelImageDto> CreateAsync(CreateHotelImageDto dto, CancellationToken cancellationToken = default);
    Task<HotelImageDto?> UpdateAsync(int id, UpdateHotelImageDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<HotelImageDto>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default);

    Task<HotelImageDto> UploadAsync(
        int hotelId,
        Stream stream,
        long fileSize,
        string fileName,
        string contentType,
        bool isPrimary,
        int sortOrder,
        CancellationToken cancellationToken = default);
}
