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
        var expectedCompany = new Company("123456789", "Test AS", "AS", "Bokmål");

        _companyService
            .GetCompanyAsync(
                Arg.Is<OrganizationNumber>(n => n.Value == "123456789"),
                Arg.Any<CancellationToken>())
            .Returns(Result.Success(expectedCompany));

        var result = await _handler.Handle(
            new GetCompanyQuery("123456789"),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expectedCompany);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("12345678")]   // 8 digits
    [InlineData("1234567890")] // 10 digits
    [InlineData("12345678a")]  // non-digit
    [InlineData("invalid")]
    public async Task Handle_WithInvalidOrganizationNumber_ShouldReturnFailureWithoutCallingService(
        string orgNumber)
    {
        var result = await _handler.Handle(
            new GetCompanyQuery(orgNumber),
            CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        await _companyService
            .DidNotReceive()
            .GetCompanyAsync(Arg.Any<OrganizationNumber>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCompanyNotFound_ShouldReturnFailureFromService()
    {
        const string error = "No company found with organization number 123456789.";

        _companyService
            .GetCompanyAsync(Arg.Any<OrganizationNumber>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Company>(error));

        var result = await _handler.Handle(
            new GetCompanyQuery("123456789"),
            CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public async Task Handle_WhenServiceFails_ShouldPropagateError()
    {
        const string error = "Failed to retrieve company data: connection refused.";

        _companyService
            .GetCompanyAsync(Arg.Any<OrganizationNumber>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Company>(error));

        var result = await _handler.Handle(
            new GetCompanyQuery("123456789"),
            CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }
}
