using CSharpFunctionalExtensions;

namespace Domain.Companies;

public sealed class OrganizationNumber : ValueObject
{
    public string Value { get; }

    private OrganizationNumber(string value) => Value = value;

    public static Result<OrganizationNumber> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<OrganizationNumber>("Organization number cannot be empty.");
        }

        if (value.Length != 9)
        {
            return Result.Failure<OrganizationNumber>("Organization number must be exactly 9 digits.");
        }

        if (!value.All(char.IsDigit))
        {
            return Result.Failure<OrganizationNumber>("Organization number must contain only digits.");
        }

        return Result.Success(new OrganizationNumber(value));
    }

    // CSharpFunctionalExtensions v3 changed the return type to IEnumerable<object>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
