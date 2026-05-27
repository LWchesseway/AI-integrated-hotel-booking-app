using DoAn.HotelParking.Core.Application.DTOs.Payment;

namespace DoAn.HotelParking.Core.Application.Interfaces.Payment;

public interface IPaymentService
{
	Task<IEnumerable<PaymentDto>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<(IEnumerable<PaymentDto> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
	Task<PaymentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<PaymentDto> CreateAsync(CreatePaymentDto dto, CancellationToken cancellationToken = default);
	Task<PaymentDto?> UpdateAsync(int id, UpdatePaymentDto dto, CancellationToken cancellationToken = default);
	Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}