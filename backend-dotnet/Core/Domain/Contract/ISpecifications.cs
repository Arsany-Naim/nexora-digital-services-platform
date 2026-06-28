using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contract
{
    //Dynamic specifications for querying entities
    public interface ISpecifications<TKey , TEntity> where TEntity: BaseEntity<TKey>
    {
        List<Expression<Func<TEntity,object>>> Includes { get; set; } // First Part
        Expression<Func<TEntity,bool>>? Criteria { get; set; }
        Expression<Func<TEntity,object>>? OrderBy { get; set; }
        Expression<Func<TEntity,object>>? OrderByDescending { get; set; }
        int Skip { get; set; }
        int Take { get; set; }
        bool IsPagination { get; set; }
    }
}
