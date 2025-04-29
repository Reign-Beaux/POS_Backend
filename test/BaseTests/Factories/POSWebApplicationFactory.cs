using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.API;

namespace BaseTests.Factories
{
    public class POSWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            //builder.ConfigureServices(services =>
            //{
            //    // Quita el DbContext registrado previamente (el que va a la BD real)
            //    var descriptor = services.SingleOrDefault(
            //        d => d.ServiceType == typeof(DbContextOptions<PosDbContext>));
            //    if (descriptor != null)
            //        services.Remove(descriptor);

            //    // Agrega el DbContext usando InMemory
            //    services.AddDbContext<PosDbContext>(options =>
            //    {
            //        options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid()); // Asegura aislamiento entre pruebas
            //    });

            //    // Opcional: asegurarse de crear la BD
            //    var sp = services.BuildServiceProvider();
            //    using var scope = sp.CreateScope();
            //    var db = scope.ServiceProvider.GetRequiredService<PosDbContext>();
            //    db.Database.EnsureCreated();
            //});

            return base.CreateHost(builder);
        }
    }
}
