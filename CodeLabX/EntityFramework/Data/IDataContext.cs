using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeLabX.EntityFramework.Data
{
    public interface IDataContext : IDisposable
    {
        void DeleteById<TEntity>(long? id) where TEntity : class;
        Task SaveChangesAsync();
        int SaveChange();
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}
