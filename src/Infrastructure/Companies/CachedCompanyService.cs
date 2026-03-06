using Application.Companies;
using CSharpFunctionalExtensions;
using Domain.Companies;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Companies;

/// <summary>
/// Caching decorator around <see cref="BrregCompanyService"/>.
/// Company registry data changes infrequently, so results are cached for one hour.
/// </summary>
internal sealed class CachedCompanyService(
    BrregCompanyService inner,
    IMemoryCache cache) : ICompanyService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    public async Task<Result<Company>> GetCompanyAsync(
        OrganizationNumber organizationNumber,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"company_{organizationNumber.Value}";

        if (cache.TryGetValue(cacheKey, out Company? cachedCompany) && cachedCompany is not null)
        {
            return Result.Success(cachedCompany);
        }

        var result = await inner.GetCompanyAsync(organizationNumber, cancellationToken);

        if (result.IsSuccess)
        {
            cache.Set(cacheKey, result.Value, CacheDuration);
        }

        return result;
    }
}
