using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Repositories {
    public abstract class BaseRepository<TEntity, TDbContext> where TEntity : class where TDbContext : DbContext {
        protected readonly TDbContext _dbContext;

        protected BaseRepository(TDbContext dbContext) {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync() {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync(int id) {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity) {
            if (entity == null) {
                throw new ArgumentNullException(nameof(entity));
            }

            await _dbContext.Set<TEntity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity) {
            if (entity == null) {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbContext.Set<TEntity>().Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(int id) {
            var entity = await _dbContext.Set<TEntity>().FindAsync(id);
            if (entity == null) {
                return false;
            }

            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> DeleteChatAsync(long id)
        {
            var entity = await _dbContext.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        protected virtual async Task<int> SaveChangesAsync() {
            return await _dbContext.SaveChangesAsync();
        }

        public virtual async Task ExecuteInTransactionAsync(Func<Task> operation) {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try {
                await operation();
                await transaction.CommitAsync();
            } catch (Exception) {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public virtual async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation) {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try {
                var result = await operation();
                await transaction.CommitAsync();
                return result;
            } catch (Exception) {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
