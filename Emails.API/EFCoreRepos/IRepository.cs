using FunctionalExtensionsLibrary.Exceptions;
using FunctionalExtensionsLIbrary.Nulls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emails.API.EFCoreRepos
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        Task<Result<IEnumerable<TEntity>>> AllAsync();
        Task<Result<Maybe<IEnumerable<TEntity>>>> AllWhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
        Task<Result<Maybe<TEntity>>> FindByKeyAsync(long id, CancellationToken cancellationToken);
        Task<Result<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken);
        Task<Result<IEnumerable<TEntity>>> InsertRangeAsync(IEnumerable<TEntity> range, CancellationToken cancellationToken);
        Task<Result<TEntity>> UpdateAsync(long id, TEntity entity, CancellationToken cancellationToken);
        Task<Result> DeleteAsync(long id, CancellationToken cancellationToken);
        Task<Result<Maybe<IEnumerable<TEntity>>>> AllWhereIncludeAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<TEntity, object>>[] includeProperties);
        Task<Result<Maybe<TEntity>>> FindByKeyIncludeAsync(long id, CancellationToken cancellationToken = default(CancellationToken),
        params Expression<Func<TEntity, object>>[] includeProperties);
    }
}
