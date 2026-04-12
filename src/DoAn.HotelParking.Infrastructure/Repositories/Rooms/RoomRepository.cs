using DoAn.HotelParking.Core.Application.Interfaces.Rooms;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.Rooms;

public class RoomRepository : GenericRepository<Room>, IRoomRepository
{
    public RoomRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Room>> GetRoomsByHotelAsync(int hotelId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Hotel)
            .Include(r => r.RoomType)
            .Where(r => r.HotelId == hotelId && !r.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Room>> GetRoomsByStatusAsync(int status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Hotel)
            .Include(r => r.RoomType)
            .Where(r => (int)r.Status == status && !r.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Room>> GetRoomsByTypeAsync(int roomTypeId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Hotel)
            .Include(r => r.RoomType)
            .Where(r => r.RoomTypeId == roomTypeId && !r.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<Room?> GetByRoomNumberAsync(int hotelId, string roomNumber, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Hotel)
            .Include(r => r.RoomType)
            .FirstOrDefaultAsync(r => r.HotelId == hotelId && r.RoomNumber == roomNumber && !r.IsDeleted, cancellationToken);
    }

    public async Task<Room?> GetRoomDetailAsync(int roomId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Hotel)
            .Include(r => r.RoomType)
            .Include(r => r.RoomImages)
            .Include(r => r.Bookings)
            .Include(r => r.Reviews)
            .FirstOrDefaultAsync(r => r.Id == roomId && !r.IsDeleted, cancellationToken);
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken = default)
    {
        // Kiểm tra xem phòng này có bất kỳ đặt phòng nào trùng lịch không
        return !await Context.Bookings
            .AnyAsync(b => b.RoomId == roomId
                && b.Status == (int)Core.Domain.Enums.BookingStatus.Confirmed
                && b.CreatedAt < checkOut
                && b.UpdatedAt > checkIn,
            cancellationToken);
    }

    public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(int hotelId, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken = default)
    {
        // Lấy danh sách các phòng ID đã được đặt trong khoảng thời gian
        var bookedRoomIds = await Context.Bookings
            .Where(b => b.Room.HotelId == hotelId
                && b.Status == (int)Core.Domain.Enums.BookingStatus.Confirmed
                && b.CreatedAt < checkOut
                && b.UpdatedAt > checkIn)
            .Select(b => b.RoomId)
            .Distinct()
            .ToListAsync(cancellationToken);

        // Lấy danh sách các phòng không có trong danh sách đã đặt
        return await DbSet
            .Include(r => r.Hotel)
            .Include(r => r.RoomType)
            .Where(r => r.HotelId == hotelId
                && !r.IsDeleted
                && !bookedRoomIds.Contains(r.Id))
            .ToListAsync(cancellationToken);
    }
}
