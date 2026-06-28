using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contract
{
    public interface IUnitOfWork
    {
        //Generate Repository for Each Entity
        IGenericRepository<TKey,TEntity> GetRepository<TKey,TEntity>() where TEntity : BaseEntity<TKey>;
        //Save Changes
        Task<int> SaveChangesAsync();
    }
}
