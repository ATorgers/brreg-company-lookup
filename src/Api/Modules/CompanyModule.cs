using Application.Companies;
using Application.Companies.GetCompany;
using Carter;
using Mediator;

namespace Api.Modules;

public class CompanyModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/companies");

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
        });
    }
}
