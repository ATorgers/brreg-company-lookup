using CSharpFunctionalExtensions;
using Domain.Companies;
using Mediator;

namespace Application.Companies.GetCompany;

public sealed class GetCompanyQueryHandler(ICompanyService companyService)
    : IQueryHandler<GetCompanyQuery, Result<Company>>
{
    public async ValueTask<Result<Company>> Handle(
        GetCompanyQuery query,
        CancellationToken cancellationToken)
    {
        var numberResult = OrganizationNumber.Create(query.OrganizationNumber);

        if (numberResult.IsFailure)
        {
            return Result.Failure<Company>(numberResult.Error);
        }

        return await companyService.GetCompanyAsync(numberResult.Value, cancellationToken);
    }
}
