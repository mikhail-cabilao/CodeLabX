using CodeLabX.EntityFramework.Data;
using CodeLabX.EntityFramework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeLabX.EntityFramework.Repository
{
    public interface IRepository
    {
        void SaveChanges();
        Task SaveChangesAsync();
        Task<IEnumerable<T>> GetAsync<T>() where T : class, IEntityContext;
        Task AddAsync<T>(T entity) where T : class, IEntityContext;
        Task<T> UpdateAsync<T>(T entity) where T : class, IEntityContext;
        Task<T> UpsertAsync<T>(T entity) where T : class, IEntityContext;
        Task<bool> DeleteAsync<T>(T entity) where T : class, IEntityContext;
    }

    public class Repository : IRepository
    {
        private readonly IDataContext dataContext;

        public Repository(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void SaveChanges()
        {
            dataContext.SaveChange();
        }

        public async Task SaveChangesAsync()
        {
            await dataContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAsync<T>() where T: class, IEntityContext
        {
            return await Task.FromResult(dataContext.Set<T>().Cast<T>());
        }

        public async Task AddAsync<T>(T entity) where T: class, IEntityContext
        {
            dataContext.Set<T>().Add(entity);
            await SaveChangesAsync();
        }

        public async Task<T> UpdateAsync<T>(T entity) where T: class, IEntityContext
        {
            var result = dataContext.Set<T>().Update(entity);
            await SaveChangesAsync();

            return result.Entity;
        }

        public async Task<T> UpsertAsync<T>(T entity) where T: class, IEntityContext
        {
            return await dataContext.Set<T>().UpsertAsync(entity, (t) => t.Id == entity.Id);
        }

        public async Task<bool> DeleteAsync<T>(T entity) where T: class, IEntityContext
        {
            try
            {
                dataContext.DeleteById<T>(entity.Id);
                await SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
