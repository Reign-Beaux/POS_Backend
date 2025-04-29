using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Web.API.Integrations
{
    public class POSWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            // Aquí puedes sobrescribir servicios para las pruebas si es necesario.
            // Por ejemplo, reemplazar la base de datos real con una en memoria:
            /*
            builder.ConfigureServices(services =>
            {
                // Elimina la configuración de DbContext de la aplicación principal
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<ApplicationDbContext>)); // Reemplaza ApplicationDbContext con tu DbContext

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Agrega un DbContext que use una base de datos en memoria
                services.AddDbContext<ApplicationDbContext>(options => // Reemplaza ApplicationDbContext con tu DbContext
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Opcional: Asegúrate de que la base de datos en memoria se cree
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>(); // Reemplaza ApplicationDbContext con tu DbContext
                    db.Database.EnsureCreated();
                }
            });
            */

            return base.CreateHost(builder);
        }
    }
}
