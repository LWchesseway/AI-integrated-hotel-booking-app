using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;

namespace DoAn.HotelParking.Core.Application.Interfaces.Hotel;

public interface IHotelRepository : IGenericRepository<Domain.Entities.Hotel.Hotel>
{
    /// <summary>
    /// Lấy khách sạn theo tên
    /// </summary>
    Task<Domain.Entities.Hotel.Hotel?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách khách sạn theo trạng thái
    /// </summary>
    Task<IEnumerable<Domain.Entities.Hotel.Hotel>> GetByStatusAsync(int status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách khách sạn theo tỉnh/thành phố
    /// </summary>
    Task<IEnumerable<Domain.Entities.Hotel.Hotel>> GetByProvinceAsync(string province, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy khách sạn cùng với danh sách phòng
    /// </summary>
    Task<Domain.Entities.Hotel.Hotel?> GetWithRoomsAsync(int hotelId, CancellationToken cancellationToken = default);
}
