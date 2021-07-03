using CodeLabX.ActiveXData;
using CodeLabX.EntityFramework.Data;
using CodeLabX.EntityFramework.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace CodeLabX.DependencyInjection
{
    public static class ServicesExtensions
    {
        public static void AddXproServices(this IServiceCollection services)
        {
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IDataContext, DataContext>();
            services.AddScoped<ISqlDatabase, SqlDatabase>();
            services.AddLogging();
        }

        public static void AddXproDataContext(this IServiceCollection services, string database, Action<SqlServerDbContextOptionsBuilder> builder)
        {
            services.AddDbContext<DataContext>(options => options.UseSqlServer(database, builder));
        }
    }
}
