using Microsoft.Extensions.DependencyInjection;
using OnlineShop.Data.Interfaces;
using OnlineShop.Data.Repositories;
using OnlineShop.Data.Services;

namespace OnlineShop.Data.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddData(this IServiceCollection services, string? connectionString)
    {
        return services
            .AddScoped<IDatabaseConnection, DapperDatabaseConnection>(
                _ => new DapperDatabaseConnection(connectionString))
            .AddScoped<IAppUserRepository, AppUserRepository>()
            .AddScoped<IAppUserService, AppUserService>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IProductService, ProductService>();
    }
}
