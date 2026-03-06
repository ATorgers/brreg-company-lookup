namespace Domain.Companies;

public sealed record Company(
    string OrganizationNumber,
    string OrganizationName,
    string CompanyType,
    string LanguageForm);
