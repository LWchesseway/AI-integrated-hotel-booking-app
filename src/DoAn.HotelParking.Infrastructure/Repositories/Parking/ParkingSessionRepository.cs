using DoAn.HotelParking.Core.Application.Interfaces.Parking;
using DoAn.HotelParking.Core.Domain.Entities.Parking;
using DoAn.HotelParking.Core.Domain.Enums;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.Parking;

public class ParkingSessionRepository : GenericRepository<ParkingSession>, IParkingSessionRepository
{
    public ParkingSessionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ParkingSession>> GetSessionsByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(ps => ps.User)
            .Include(ps => ps.QrUser)
            .Include(ps => ps.VerifiedByUser)
            .Include(ps => ps.LicensePlateLogs)
            .Where(ps => ps.UserId == userId)
            .OrderByDescending(ps => ps.CheckInTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ParkingSession>> GetActiveSessions(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(ps => ps.User)
            .Include(ps => ps.QrUser)
            .Include(ps => ps.LicensePlateLogs)
            .Where(ps => ps.Status == ParkingSessionStatus.CheckedIn)
            .OrderBy(ps => ps.CheckInTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ParkingSession>> GetSessionsByStatusAsync(int status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(ps => ps.User)
            .Include(ps => ps.QrUser)
            .Include(ps => ps.LicensePlateLogs)
            .Where(ps => (int)ps.Status == status)
            .OrderByDescending(ps => ps.CheckInTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<ParkingSession?> GetSessionDetailAsync(int sessionId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(ps => ps.User)
            .Include(ps => ps.QrUser)
            .Include(ps => ps.VerifiedByUser)
            .Include(ps => ps.LicensePlateLogs)
            .FirstOrDefaultAsync(ps => ps.Id == sessionId, cancellationToken);
    }

    public async Task<ParkingSession?> GetActiveSessionByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(ps => ps.User)
            .Include(ps => ps.QrUser)
            .Include(ps => ps.LicensePlateLogs)
            .FirstOrDefaultAsync(ps => ps.UserId == userId
                && ps.Status == ParkingSessionStatus.CheckedIn,
            cancellationToken);
    }
}
