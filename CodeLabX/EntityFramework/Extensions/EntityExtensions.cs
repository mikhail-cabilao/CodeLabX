using System.Linq;
using System.Threading.Tasks;

namespace CodeLabX.EntityFramework.Extensions
{
    public static class EntityExtensions
    {
        public static async Task<TEntity> ResolveEntity<TEntity>(this TEntity entiy, TEntity mapper) where TEntity : class, IEntityContext
        {
            var properties = entiy.GetType()
                .GetProperties()
                .Where(d => d.GetValue(entiy) != null && !(d.Name == "Id" || d.Name == "CreatedDate" || d.Name == "ModifiedDate"))
                .ToList();

            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(entiy);
                mapper.GetType().GetProperty(property.Name).SetValue(mapper, propertyValue);
            }

            return await Task.Run(() => { return mapper; });
        }
    }
}
