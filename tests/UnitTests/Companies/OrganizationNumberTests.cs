using Domain.Companies;
using Shouldly;
using Xunit;

namespace UnitTests.Companies;

public class OrganizationNumberTests
{
    [Theory]
    [InlineData("812345678")] // starts with 8
    [InlineData("974760673")] // starts with 9
    public void Create_WithValidNumber_ShouldSucceed(string value)
    {
        var result = OrganizationNumber.Create(value);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyOrNull_ShouldFail(string? value)
    {
        var result = OrganizationNumber.Create(value);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("12345678")]   // 8 digits — too short
    [InlineData("1234567890")] // 10 digits — too long
    public void Create_WithWrongLength_ShouldFail(string value)
    {
        var result = OrganizationNumber.Create(value);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("9 digits");
    }

    [Theory]
    [InlineData("12345678a")]
    [InlineData("1234-5678")]
    [InlineData("abc456789")]
    [InlineData("123 56789")]
    public void Create_WithNonDigitCharacters_ShouldFail(string value)
    {
        var result = OrganizationNumber.Create(value);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("digits");
    }

    [Theory]
    [InlineData("123456789")] // starts with 1
    [InlineData("523456789")] // starts with 5
    [InlineData("723456789")] // starts with 7
    public void Create_WithNumberNotStartingWith8Or9_ShouldFail(string value)
    {
        var result = OrganizationNumber.Create(value);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("8 or 9");
    }

    [Fact]
    public void Create_WithSameValue_ShouldBeEqual()
    {
        var first = OrganizationNumber.Create("974760673").Value;
        var second = OrganizationNumber.Create("974760673").Value;

        first.ShouldBe(second);
    }

    [Fact]
    public void Create_WithDifferentValues_ShouldNotBeEqual()
    {
        var first = OrganizationNumber.Create("974760673").Value;
        var second = OrganizationNumber.Create("812345678").Value;

        first.ShouldNotBe(second);
    }

    [Fact]
    public void ToString_ShouldReturnNumericValue()
    {
        var orgNumber = OrganizationNumber.Create("974760673").Value;

        orgNumber.ToString().ShouldBe("974760673");
    }
}
