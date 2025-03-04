using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services {
    public abstract class BaseService<TEntity, TRepository>
        where TEntity : class
        where TRepository : BaseRepository<TEntity, TDbContext> {

        protected readonly TRepository _repository;

        protected BaseService(TRepository repository) {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync() {
            return await _repository.GetAllAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync(int id) {
            return await _repository.GetByIdAsync(id);
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity) {
            return await _repository.AddAsync(entity);
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity) {
            return await _repository.UpdateAsync(entity);
        }

        public virtual async Task<bool> DeleteAsync(int id) {
            return await _repository.DeleteAsync(id);
        }

        public virtual async Task ExecuteInTransactionAsync(Func<Task> operation) {
            await _repository.ExecuteInTransactionAsync(operation);
        }

        public virtual async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation) {
            return await _repository.ExecuteInTransactionAsync(operation);
        }
    }
}
