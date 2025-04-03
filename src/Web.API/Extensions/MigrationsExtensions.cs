using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Web.API.Extensions
{
    public static class MigrationsExtensions
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<PosDbContext>();

            dbContext.Database.Migrate();
        }
    }
}
