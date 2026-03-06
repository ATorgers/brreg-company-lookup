using Domain.Companies;
using Shouldly;
using Xunit;

namespace UnitTests.Companies;

public class OrganizationNumberTests
{
    [Fact]
    public void Create_WithValidNineDigitNumber_ShouldSucceed()
    {
        var result = OrganizationNumber.Create("123456789");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe("123456789");
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

    [Fact]
    public void Create_WithSameValue_ShouldBeEqual()
    {
        var first = OrganizationNumber.Create("123456789").Value;
        var second = OrganizationNumber.Create("123456789").Value;

        first.ShouldBe(second);
    }

    [Fact]
    public void Create_WithDifferentValues_ShouldNotBeEqual()
    {
        var first = OrganizationNumber.Create("123456789").Value;
        var second = OrganizationNumber.Create("987654321").Value;

        first.ShouldNotBe(second);
    }

    [Fact]
    public void ToString_ShouldReturnNumericValue()
    {
        var orgNumber = OrganizationNumber.Create("123456789").Value;

        orgNumber.ToString().ShouldBe("123456789");
    }
}
