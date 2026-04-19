using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Payment;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Payment;
using PaymentEntity = DoAn.HotelParking.Core.Domain.Entities.Booking.Payment;

namespace DoAn.HotelParking.Core.Application.Services.Payment;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PaymentService(
        IPaymentRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _paymentRepository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _paymentRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<PaymentDto>>(entities);
    }

    public async Task<(IEnumerable<PaymentDto> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _paymentRepository.GetPagedAsync(pageIndex, pageSize, null, cancellationToken);
        return (_mapper.Map<IEnumerable<PaymentDto>>(items), totalCount);
    }

    public async Task<PaymentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _paymentRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? default : _mapper.Map<PaymentDto>(entity);
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<PaymentEntity>(dto);
        await _paymentRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<PaymentDto>(entity);
    }

    public async Task<PaymentDto?> UpdateAsync(int id, UpdatePaymentDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _paymentRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return default;
        }

        _mapper.Map(dto, entity);
        _paymentRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<PaymentDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _paymentRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _paymentRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}