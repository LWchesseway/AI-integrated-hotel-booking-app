using AutoMapper;
using DoAn.HotelParking.Core.Application.Interfaces.Base;

namespace DoAn.HotelParking.Core.Application.Services.Base;

public class CrudService<TEntity, TDto, TCreateDto, TUpdateDto> : ICrudService<TDto, TCreateDto, TUpdateDto>
    where TEntity : class
{
    protected readonly IGenericRepository<TEntity> Repository;
    protected readonly IUnitOfWork UnitOfWork;
    protected readonly IMapper Mapper;

    public CrudService(
        IGenericRepository<TEntity> repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        Repository = repository;
        UnitOfWork = unitOfWork;
        Mapper = mapper;
    }

    public virtual async Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await Repository.GetAllAsync(cancellationToken);
        return Mapper.Map<IEnumerable<TDto>>(entities);
    }

    public virtual async Task<(IEnumerable<TDto> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await Repository.GetPagedAsync(pageIndex, pageSize, null, cancellationToken);
        return (Mapper.Map<IEnumerable<TDto>>(items), totalCount);
    }

    public virtual async Task<TDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? default : Mapper.Map<TDto>(entity);
    }

    public virtual async Task<TDto> CreateAsync(TCreateDto dto, CancellationToken cancellationToken = default)
    {
        var entity = Mapper.Map<TEntity>(dto);
        await Repository.AddAsync(entity, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        return Mapper.Map<TDto>(entity);
    }

    public virtual async Task<TDto?> UpdateAsync(int id, TUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return default;
        }

        Mapper.Map(dto, entity);
        Repository.Update(entity);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        return Mapper.Map<TDto>(entity);
    }

    public virtual async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        Repository.Remove(entity);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}