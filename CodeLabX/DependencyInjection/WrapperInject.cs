using CodeLabX.EntityFramework.Data;
using CodeLabX.EntityFramework.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace CodeLabX.DependencyInjection
{
    public static class WrapperInject
    {
        public static void ServicesInject(IServiceCollection services)
        {
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IDataContext, DataContext>();
        }
    }
}
