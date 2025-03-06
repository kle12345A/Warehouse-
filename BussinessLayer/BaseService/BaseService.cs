using DataAccessLayer.BaseRepository;
using System.Linq.Expressions;

namespace BussinessLayer.BaseService
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        protected readonly IBaseRepository<T> _baseRepository;

        public BaseService(IBaseRepository<T> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _baseRepository.AddAsync(entity);
            await _baseRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _baseRepository.GetByIdAsync(id);
            if (entity == null)
                return false;

            await _baseRepository.DeleteAsync(entity);
            await _baseRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _baseRepository.FindAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _baseRepository.GetAllAsync();
        }

        public IQueryable<T> GetAllQueryable()
        {
            return _baseRepository.GetQuery();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _baseRepository.GetByIdAsync(id);
        }

        public IQueryable<T> GetQuery()
        {
            return _baseRepository.GetQuery();
        }

        public IQueryable<T> GetQuery(Expression<Func<T, bool>> where)
        {
            return _baseRepository.GetQuery(where);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _baseRepository.UpdateAsync(entity);
            await _baseRepository.SaveChangesAsync();
        }
    }
}
