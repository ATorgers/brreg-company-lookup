using CSharpFunctionalExtensions;
using Domain.Companies;
using Mediator;

namespace Application.Companies.GetCompany;

public sealed class GetCompanyQueryHandler(ICompanyService companyService)
    : IQueryHandler<GetCompanyQuery, Result<Company, CompanyError>>
{
    public async ValueTask<Result<Company, CompanyError>> Handle(
        GetCompanyQuery query,
        CancellationToken cancellationToken)
    {
        var numberResult = OrganizationNumber.Create(query.OrganizationNumber);

        if (numberResult.IsFailure)
        {
            return Result.Failure<Company, CompanyError>(
                new CompanyError(CompanyErrorType.ValidationError, numberResult.Error));
        }

        return await companyService.GetCompanyAsync(numberResult.Value, cancellationToken);
    }
}
