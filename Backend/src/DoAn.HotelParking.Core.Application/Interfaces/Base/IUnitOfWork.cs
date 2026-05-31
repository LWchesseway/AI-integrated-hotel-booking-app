namespace DoAn.HotelParking.Core.Application.Interfaces.Base;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}