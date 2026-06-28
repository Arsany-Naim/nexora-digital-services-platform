using Domain.Contract;
using Domain.Entities;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Context;

namespace Persistence.Repositories
{
    public class GeericRepository<TKey, TEntity>(StoreDbContext _context): IGenericRepository<TKey, TEntity> where TEntity : BaseEntity<TKey>
    {
        public async Task<IEnumerable<TEntity>> GetAllAsync(bool changeTracker = false)
        {
            if (typeof(TEntity) == typeof(Product))
            {
                return changeTracker ?
                    await _context.Products.Include(p => p.Brand).Include(p => p.Type).ToListAsync() as IEnumerable<TEntity> :
                    await _context.Products.Include(p => p.Brand).Include(p => p.Type).AsNoTracking().ToListAsync() as IEnumerable<TEntity>;
            }
            return changeTracker ?
              await _context.Set<TEntity>().ToListAsync() :
              await _context.Set<TEntity>().AsNoTracking().ToListAsync();

        }
        public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TKey, TEntity> spec, bool changeTracker = false)
        {
           return await ApplySpecifications(spec).ToListAsync();
        }
        public async Task<TEntity?> GetAsync(TKey key)
        {
            if (typeof(TEntity) == typeof(Product))
            {
                return await _context.Products.Include(p=> p.Brand).Include(p=> p.Type).FirstOrDefaultAsync(p=> p.Id == key as int?) as TEntity;
            }
            return await _context.Set<TEntity>().FindAsync(key);
        }
        public async Task<TEntity?> GetAsync(ISpecifications<TKey, TEntity> spec, TKey key)
        {
            return await ApplySpecifications(spec).FirstOrDefaultAsync();
        }
        public async Task<TEntity?> GetAsync(ISpecifications<TKey, TEntity> spec)
        {
            return await ApplySpecifications(spec).FirstOrDefaultAsync();
        }
        public async Task AddAsync(TEntity entity)
        {
            await _context.AddAsync(entity);
        }
        public void Update(TEntity entity)
        {
            _context.Update(entity);
        }
        public void Delete(TEntity entity)
        {
            _context.Remove(entity);
        }
        public async Task<int> GetCountAsync(ISpecifications<TKey, TEntity> spec)
        {
            return await ApplySpecifications(spec).CountAsync();
        }
        private IQueryable<TEntity> ApplySpecifications(ISpecifications<TKey, TEntity> spec)
        {
            return SpecificationsEvaluator.GetQuery(_context.Set<TEntity>(), spec);
        }

    }
}
