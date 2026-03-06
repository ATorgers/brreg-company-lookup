using Application.Companies.GetCompany;
using Carter;
using Mediator;

namespace Api.Modules;

public class CompanyModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/companies")
                       .WithTags("Companies")
                       .WithOpenApi();

        group.MapGet("/{organizationNumber}", async (
            string organizationNumber,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new GetCompanyQuery(organizationNumber),
                cancellationToken);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(
                    detail: result.Error,
                    statusCode: StatusCodes.Status422UnprocessableEntity);
        })
        .WithName("GetCompany")
        .WithSummary("Look up a Norwegian company by its 9-digit organization number")
        .Produces<Domain.Companies.Company>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity);
    }
}
