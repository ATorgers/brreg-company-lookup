using CSharpFunctionalExtensions;
using Domain.Companies;
using Mediator;

namespace Application.Companies.GetCompany;

public sealed record GetCompanyQuery(string OrganizationNumber) : IQuery<Result<Company, CompanyError>>;
