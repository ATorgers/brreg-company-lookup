using CSharpFunctionalExtensions;
using Domain.Companies;

namespace Application.Companies;

public interface ICompanyService
{
    Task<Result<Company>> GetCompanyAsync(
        OrganizationNumber organizationNumber,
        CancellationToken cancellationToken = default);
}
