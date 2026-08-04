using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BudganInfra
{
    public static class MigrationsExtensions
    {
        public static IServiceProvider ApplyMigrations(this IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<DataContext>().Database.Migrate();
            }

            return services;
        }
    }
}