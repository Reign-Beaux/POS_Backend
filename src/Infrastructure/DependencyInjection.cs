using Application.Consts;
using Application.Interfaces.Context;
using Application.Interfaces.UnitOfWorks;
using Domain.Entities.Articles;
using Domain.Entities.ArticleTypes;
using Domain.Entities.Brands;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services
                .AddAdapters()
                .AddPersistence(configuration)
                .AddServices();

            return services;
        }

        private static IServiceCollection AddAdapters(this IServiceCollection services)
        {
            return services;
        }

        private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PosDbContext>(options => options.UseSqlServer(configuration.GetConnectionString(ConnectionStrings.PosDB)));
            services.AddScoped<IPosDbContext>(sp => sp.GetRequiredService<PosDbContext>());
            services.AddScoped<IPosDbUnitOfWork, PosDbUnitOfWork>();

            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<IArticleTypeRepository, ArticleTypeRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {

            return services;
        }
    }
}
