using Application.Companies;
using Application.Companies.GetCompany;
using CSharpFunctionalExtensions;
using Domain.Companies;
using NSubstitute;
using Shouldly;
using Xunit;

namespace UnitTests.Companies;

public class GetCompanyQueryHandlerTests
{
    private readonly ICompanyService _companyService = Substitute.For<ICompanyService>();
    private readonly GetCompanyQueryHandler _handler;

    public GetCompanyQueryHandlerTests()
    {
        _handler = new GetCompanyQueryHandler(_companyService);
    }

    [Fact]
    public async Task Handle_WithValidOrganizationNumber_ShouldReturnCompany()
    {
        var expectedCompany = new Company("974760673", "Test AS", "AS", "Bokmål");

        _companyService
            .GetCompanyAsync(
                Arg.Is<OrganizationNumber>(n => n.Value == "974760673"),
                Arg.Any<CancellationToken>())
            .Returns(Result.Success<Company, CompanyError>(expectedCompany));

        var result = await _handler.Handle(
            new GetCompanyQuery("974760673"),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expectedCompany);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("12345678")]   // too short
    [InlineData("1234567890")] // too long
    [InlineData("12345678a")]  // non-digit
    [InlineData("invalid")]
    [InlineData("123456789")]  // does not start with 8 or 9
    public async Task Handle_WithInvalidOrganizationNumber_ShouldReturnValidationErrorWithoutCallingService(
        string orgNumber)
    {
        var result = await _handler.Handle(
            new GetCompanyQuery(orgNumber),
            CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(CompanyErrorType.ValidationError);
        await _companyService
            .DidNotReceive()
            .GetCompanyAsync(Arg.Any<OrganizationNumber>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCompanyNotFound_ShouldReturnNotFoundError()
    {
        const string message = "No company found with organization number 974760673.";
        var error = new CompanyError(CompanyErrorType.NotFound, message);

        _companyService
            .GetCompanyAsync(Arg.Any<OrganizationNumber>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Company, CompanyError>(error));

        var result = await _handler.Handle(
            new GetCompanyQuery("974760673"),
            CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(CompanyErrorType.NotFound);
        result.Error.Message.ShouldBe(message);
    }

    [Fact]
    public async Task Handle_WhenServiceUnavailable_ShouldReturnServiceUnavailableError()
    {
        const string message = "Failed to retrieve company data: connection refused.";
        var error = new CompanyError(CompanyErrorType.ServiceUnavailable, message);

        _companyService
            .GetCompanyAsync(Arg.Any<OrganizationNumber>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Company, CompanyError>(error));

        var result = await _handler.Handle(
            new GetCompanyQuery("974760673"),
            CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(CompanyErrorType.ServiceUnavailable);
        result.Error.Message.ShouldBe(message);
    }
}
