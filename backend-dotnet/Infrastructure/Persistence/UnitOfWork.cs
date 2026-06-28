using Domain.Contract;
using Domain.Entities;
using Persistence.Data.Context;
using Persistence.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class UnitOfWork(StoreDbContext _context) : IUnitOfWork
    {
        //private Dictionary<string, object> _repositories = new Dictionary<string, object>();
        private ConcurrentDictionary<string, object> _repositories = new ConcurrentDictionary<string, object>();
        public IGenericRepository<TKey, TEntity> GetRepository<TKey, TEntity>() where TEntity : BaseEntity<TKey>
        {

            //if (!_repositories.ContainsKey(typeof(TEntity).Name))
            //{
            //    var repository = new GeericRepository<TKey, TEntity>(_context);
            //    _repositories.Add(typeof(TEntity).Name, repository); 
            //}
            //return (IGenericRepository<TKey, TEntity>) _repositories[typeof(TEntity).Name];
            return (IGenericRepository<TKey, TEntity>) _repositories.GetOrAdd(typeof(TEntity).Name, new GeericRepository<TKey, TEntity>(_context));
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
