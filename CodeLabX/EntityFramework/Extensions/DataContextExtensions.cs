using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq;
using LinqKit;

namespace CodeLabX.EntityFramework.Extensions
{
    public static class DataAccessExtensions
    {
        public static async Task<TEntity> UpsertAsync<TEntity>(this DbSet<TEntity> dbSet, TEntity entity, Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            var matched = await dbSet.AsExpandable().FirstOrDefaultAsync(expression);

            if (matched == null)
            {
                dbSet.Add(entity);
                return entity;
            }

            dbSet.Update(entity);
            return matched;
        }
    }
}
