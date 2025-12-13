
using Microsoft.EntityFrameworkCore;
using WebShop1.Data;

namespace WebShop1.Models
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected AppDbContext _context {  get; set; }
        private DbSet<T> _dbSet { get; set; }
        public Repository(AppDbContext context){
            _context = context;
            _dbSet = context.Set<T>();
        
        }
        public Task AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public Task<T> GetByIdAsync(int id, QueryOptions<T> options)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
