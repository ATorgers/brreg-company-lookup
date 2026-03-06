using Application.Companies;
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

            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            return result.Error.Type switch
            {
                CompanyErrorType.ValidationError => Results.Problem(
                    detail: result.Error.Message,
                    statusCode: StatusCodes.Status422UnprocessableEntity),
                CompanyErrorType.NotFound => Results.Problem(
                    detail: result.Error.Message,
                    statusCode: StatusCodes.Status404NotFound),
                CompanyErrorType.ServiceUnavailable => Results.Problem(
                    detail: result.Error.Message,
                    statusCode: StatusCodes.Status503ServiceUnavailable),
                _ => Results.Problem(
                    detail: result.Error.Message,
                    statusCode: StatusCodes.Status500InternalServerError),
            };
        })
        .WithName("GetCompany")
        .WithSummary("Look up a Norwegian company by its 9-digit organization number")
        .Produces<Domain.Companies.Company>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
        .ProducesProblem(StatusCodes.Status503ServiceUnavailable)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
