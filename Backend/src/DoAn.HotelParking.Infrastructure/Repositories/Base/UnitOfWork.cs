using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Infrastructure.Data;

namespace DoAn.HotelParking.Infrastructure.Repositories.Base;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}