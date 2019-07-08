using Emails.API.EFCoreContext;
using Emails.Domain.Aggregates.Base;
using FunctionalExtensionsLibrary.Exceptions;
using FunctionalExtensionsLIbrary.Nulls;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emails.API.EFCoreRepos
{
    public abstract class Repository<TEntity> : IDisposable, IRepository<TEntity> where TEntity : AggregateRoot
    {
        private bool _disposed;
        private EmailsContext _context;

        public EmailsContext Context => _context;

        public Repository(EmailsContext context)
        {
            _context = context;
        }


        public async Task<Result<IEnumerable<TEntity>>> AllAsync()
        {
            try
            {
                return Result.Ok<IEnumerable<TEntity>>(await _context.Set<TEntity>().ToListAsync());
            }
            catch (Exception ex)
            {
                return Result.Fail<IEnumerable<TEntity>>(ex.Message, ex.StackTrace);
            }
        }

        public async Task<Result<Maybe<IEnumerable<TEntity>>>> AllWhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return Result.Ok<Maybe<IEnumerable<TEntity>>>(await _context
                    .Set<TEntity>()
                    .AsNoTracking()
                    .Where(predicate)
                    .ToListAsync(cancellationToken));
            }
            catch (Exception ex)
            {
                return Result.Fail<Maybe<IEnumerable<TEntity>>>(ex.Message, ex.StackTrace);
            }
        }

        public async Task<Result<Maybe<IEnumerable<TEntity>>>> AllWhereIncludeAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            try
            {
                IQueryable<TEntity> query = GetAllIncluding(includeProperties);
                return Result.Ok<Maybe<IEnumerable<TEntity>>>(await query.Where(predicate).ToListAsync());
            }
            catch (Exception ex)
            {
                return Result.Fail<Maybe<IEnumerable<TEntity>>>(ex.Message, ex.StackTrace);
            }
        }

        private IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> queryable = _context.Set<TEntity>().AsNoTracking();

            return includeProperties.Aggregate(queryable, (current, includeProperty) =>

                current.Include(includeProperty)
            );
        }

        public async Task<Result> DeleteAsync(long id, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Result<Maybe<TEntity>> deleteResult = await FindByKeyAsync(id, cancellationToken);



                if (deleteResult.IsSuccess)
                {

                    if (deleteResult.Value.HasValue)
                    {
                        _context
                        .Set<TEntity>()
                        .Remove(deleteResult.Value.Value);

                        return Result.Ok();
                    }
                    else
                    {
                        return Result.Fail($"Entity {typeof(TEntity).Name} with Id = {id} was not found in the database");
                    }
                }
                else
                {
                    return deleteResult;
                }
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message, ex.StackTrace);
            }
        }


        public async Task<Result<Maybe<TEntity>>> FindByKeyIncludeAsync(long id, CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            try
            {
                ParameterExpression item = Expression.Parameter(typeof(TEntity), "entity");
                MemberExpression prop = Expression.Property(item, "Id");
                ConstantExpression value = Expression.Constant(id);
                BinaryExpression equal = Expression.Equal(prop, value);

                var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, item);

                IQueryable<TEntity> query = _context.Set<TEntity>().Where(lambda);

                IQueryable<TEntity> withIncluded = includeProperties.Aggregate(query, (current, includeProperty) =>

                    current.Include(includeProperty)
                );


                return Result.Ok<Maybe<TEntity>>(await withIncluded.SingleOrDefaultAsync());
            }
            catch (Exception ex)
            {
                return Result.Fail<Maybe<TEntity>>(ex.Message, ex.StackTrace);
            }
        }

        public async Task<Result<Maybe<TEntity>>> FindByKeyAsync(long id, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                ParameterExpression item = Expression.Parameter(typeof(TEntity), "entity");
                MemberExpression prop = Expression.Property(item, "Id");
                ConstantExpression value = Expression.Constant(id);
                BinaryExpression equal = Expression.Equal(prop, value);

                var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, item);

                var x = _context
                    .Set<TEntity>().ToList();
                return Result.Ok<Maybe<TEntity>>(await _context
                    .Set<TEntity>()
                    .SingleOrDefaultAsync(lambda, cancellationToken));
            }
            catch (Exception ex)
            {
                return Result.Fail<Maybe<TEntity>>(ex.Message, ex.StackTrace);
            }
        }

        public async Task<Result<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                _context.Set<TEntity>().Add(entity);
                await _context.SaveChangesAsync();
                return Result.Ok<TEntity>(entity);
            }
            catch (Exception ex)
            {
                return Result.Fail<TEntity>(ex.Message, ex.StackTrace);
            }
        }

        public async Task<Result<IEnumerable<TEntity>>> InsertRangeAsync(IEnumerable<TEntity> range, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                List<TEntity> entitiesToInsert = range.ToList();
                _context.Set<TEntity>().AddRange(entitiesToInsert);
                await _context.SaveChangesAsync();

                return Result.Ok<IEnumerable<TEntity>>(entitiesToInsert);
            }
            catch (Exception ex)
            {
                return Result.Fail<IEnumerable<TEntity>>(ex.Message, ex.StackTrace);
            }
        }

        public async Task<Result<TEntity>> UpdateAsync(long id, TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Result<Maybe<TEntity>> toUpdate = await FindByKeyAsync(id);

                if (toUpdate.IsSuccess)
                {
                    if (toUpdate.Value.HasValue)
                    {
                        Context.Entry(toUpdate.Value.Value).CurrentValues.SetValues(entity);
                        await _context.SaveChangesAsync();

                        return Result.Ok<TEntity>(toUpdate.Value.Value);
                    }
                    else
                    {
                        return Result.Fail<TEntity>($"Entity {typeof(TEntity).Name} with Id = {id} does not exists in the database");
                    }
                }
                else
                {
                    return Result.Fail<TEntity>(toUpdate.Error);
                }
            }
            catch (Exception ex)
            {
                return Result.Fail<TEntity>(ex.Message, ex.StackTrace);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {

            if (_disposed)
                return;

            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }

            _disposed = true;
        }

        ~Repository()
        {
            Dispose(false);
        }
    }
}
