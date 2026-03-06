using Application.Companies;
using Infrastructure.Companies;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string brregBaseUrl)
    {
        services.AddMemoryCache();

        services.AddHttpClient<BrregCompanyService>(client =>
        {
            client.BaseAddress = new Uri(brregBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        // Register the caching decorator as the ICompanyService implementation.
        // BrregCompanyService is injected into the decorator, not resolved as ICompanyService,
        // so there is no circular dependency.
        services.AddTransient<ICompanyService>(sp =>
        {
            var brregService = sp.GetRequiredService<BrregCompanyService>();
            var cache = sp.GetRequiredService<IMemoryCache>();
            return new CachedCompanyService(brregService, cache);
        });

        return services;
    }
}
