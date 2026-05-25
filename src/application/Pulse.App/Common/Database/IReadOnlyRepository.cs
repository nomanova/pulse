using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pulse.App.Common.Database.Specifications;
using Pulse.App.Common.Database.Specifications.Base;

namespace Pulse.App.Common.Database;

public interface IReadOnlyRepository<TEntity>
{
    Task<List<TEntity>> Search(
        Specification<TEntity> specification,
        CancellationToken cancellationToken = default);

    Task<uint> Count(
        Specification<TEntity> specification,
        CancellationToken cancellationToken = default);
    
    Task<TEntity?> SearchOne(
        Specification<TEntity> specification,
        CancellationToken cancellationToken = default);

    Task<TEntity?> SearchOne(
        Specification<TEntity> specification,
        IOrderBySpecification<TEntity> orderBy,
        CancellationToken cancellationToken = default);
    
    Task<SearchResult<TEntity>> SearchCursor(
        Specification<TEntity> searchBy,
        IOrderBySpecification<TEntity> orderBy,
        uint pageSize,
        Specification<TEntity>? searchLast = null,
        CancellationToken cancellationToken = default);
}