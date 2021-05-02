using CodeLabX.EntityFramework.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace CodeLabX.Bootstrap
{
    public static class Bootstrapping
    {
        public static void InitializeHostBuilder(string[] arge, Func<string[], IHostBuilder> hostBuilder)
        {
            var host = hostBuilder(arge).Build();
            CreateDbIfNotExists(host);

            host.Run();
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<DataContext>();
                    context.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
