using CSharpFunctionalExtensions;
using Domain.Companies;

namespace Application.Companies;

public interface ICompanyService
{
    Task<Result<Company, CompanyError>> GetCompanyAsync(
        OrganizationNumber organizationNumber,
        CancellationToken cancellationToken = default);
}
