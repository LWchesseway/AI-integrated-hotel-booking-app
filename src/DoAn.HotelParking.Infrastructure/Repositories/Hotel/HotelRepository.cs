using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.Hotel;

public class HotelRepository : GenericRepository<Domain.Entities.Hotel.Hotel>, IHotelRepository
{
    public HotelRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Domain.Entities.Hotel.Hotel?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(h => h.Name == name && !h.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Hotel.Hotel>> GetByStatusAsync(int status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(h => (int)h.Status == status && !h.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Hotel.Hotel>> GetByProvinceAsync(string province, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(h => h.Province == province && !h.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Hotel.Hotel?> GetWithRoomsAsync(int hotelId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.Id == hotelId && !h.IsDeleted, cancellationToken);
    }
}
