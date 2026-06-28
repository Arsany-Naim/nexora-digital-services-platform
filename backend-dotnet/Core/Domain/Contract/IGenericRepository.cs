using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contract
{
    public interface IGenericRepository<TKey , TEntity> where TEntity : BaseEntity<TKey>
    {
        Task<IEnumerable<TEntity>> GetAllAsync(bool changeTracker = false);
        Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TKey,TEntity> spec,bool changeTracker = false);
        Task<TEntity?> GetAsync(TKey key);
        Task<TEntity?> GetAsync(ISpecifications<TKey,TEntity> spec,TKey key);
        Task<TEntity?> GetAsync(ISpecifications<TKey,TEntity> spec);
        Task<int> GetCountAsync(ISpecifications<TKey,TEntity> spec);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
