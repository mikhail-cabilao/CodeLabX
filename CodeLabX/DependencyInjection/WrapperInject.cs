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
    public static class WrapperInject
    {
        public static void ServicesInject(IServiceCollection services)
        {
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IDataContext, DataContext>();
            services.AddScoped<ISqlDatabase, SqlDatabase>();
            services.AddSingleton<ILogger, Logger<SqlDatabase>>();
        }

        public static void DbContextInject(IServiceCollection services, string database, Action<SqlServerDbContextOptionsBuilder> builder)
        {
            Configuration.ConnectionString = database;
            services.AddDbContext<DataContext>(options => options.UseSqlServer(database, builder));
        }
    }
}
