using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccessLayer.BaseRepository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly WarehouseDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(WarehouseDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // Thêm một bản ghi mới
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveChangesAsync();
        }

        // Xóa bản ghi dựa trên đối tượng
        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await SaveChangesAsync();
        }

        // Xóa bản ghi dựa trên ID
        public async Task DeleteByIdAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                await DeleteAsync(entity);
            }
        }

        // Tìm bản ghi theo điều kiện
        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        // Lấy tất cả các bản ghi
        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // Lấy bản ghi theo ID
        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public IQueryable<T> GetQuery()
        {
            return _dbSet;
        }


        public IQueryable<T> GetQuery(Expression<Func<T, bool>> where)
        {
            return _dbSet.Where(where);
        }

        // Lưu các thay đổi vào cơ sở dữ liệu
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Cập nhật bản ghi
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await SaveChangesAsync();
        }
    }
}
