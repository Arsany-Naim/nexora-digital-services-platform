using Domain.Contract;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public static class SpecificationsEvaluator
    {
        public static IQueryable<TEntity> GetQuery<TKey,TEntity>(IQueryable<TEntity> inputQuery,ISpecifications<TKey,TEntity> spec) where TEntity : BaseEntity<TKey>
        {
            var query = inputQuery;
            if (spec.Criteria is not null)
            {
                query = query.Where(spec.Criteria);
            }
            if(spec.OrderBy is not null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            if(spec.OrderByDescending is not null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }
            if(spec.IsPagination)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }
            query = spec.Includes.Aggregate(query, (query, include) => query.Include(include));
            return query;
        }
    }
}
