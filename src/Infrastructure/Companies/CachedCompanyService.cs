using Application.Companies;
using CSharpFunctionalExtensions;
using Domain.Companies;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Companies;

/// <summary>
/// Caching decorator around <see cref="BrregCompanyService"/>.
/// Company registry data changes infrequently, so results are cached for one hour.
/// Only successful lookups are cached — errors are never cached.
/// </summary>
internal sealed class CachedCompanyService(
    BrregCompanyService inner,
    IMemoryCache cache) : ICompanyService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    public async Task<Result<Company, CompanyError>> GetCompanyAsync(
        OrganizationNumber organizationNumber,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"company_{organizationNumber.Value}";

        if (cache.TryGetValue(cacheKey, out Company? cachedCompany) && cachedCompany is not null)
        {
            return Result.Success<Company, CompanyError>(cachedCompany);
        }

        var result = await inner.GetCompanyAsync(organizationNumber, cancellationToken);

        if (result.IsSuccess)
        {
            cache.Set(cacheKey, result.Value, CacheDuration);
        }

        return result;
    }
}
