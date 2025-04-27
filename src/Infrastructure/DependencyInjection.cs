using Application.Constants;
using Application.Interfaces.Caching;
using Application.Interfaces.Context;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWorks;
using Domain.Entities.Articles;
using Domain.Entities.ArticleTypes;
using Domain.Entities.Brands;
using Domain.Entities.Translations;
using Infrastructure.Caching.Localization;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.UnitOfWorks;
using Infrastructure.Services;
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
            services.AddEnyimMemcached(options => configuration.GetSection("EnyimMemcached").Bind(options));
            services
                .AddCaching()
                .AddPersistence(configuration)
                .AddServices();

            return services;
        }

        private static IServiceCollection AddCaching(this IServiceCollection services)
        {
            services.AddScoped<ILocalizationCached, LocalizationCached>();

            return services;
        }

        private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PosDbContext>(options => options.UseSqlServer(configuration.GetConnectionString(ConnectionStrings.PosDB)));
            services.AddDbContext<PosUtilsDbContext>(options => options.UseSqlServer(configuration.GetConnectionString(ConnectionStrings.PosUtilsDB)));

            services.AddScoped<IPosDbContext>(sp => sp.GetRequiredService<PosDbContext>());
            services.AddScoped<IPosUtilsDbContext>(sp => sp.GetRequiredService<PosUtilsDbContext>());

            services.AddScoped<IPosDbUnitOfWork, PosDbUnitOfWork>();
            services.AddScoped<IPosUtilsDbUnitOfWork, PosUtilsDbUnitOfWork>();

            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<IArticleTypeRepository, ArticleTypeRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<ITranslationRepository, TranslationRepository>();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(ILoggingMessagesService<>), typeof(LoggingMessagesService<>));

            return services;
        }
    }
}
