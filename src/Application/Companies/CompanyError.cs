namespace Application.Companies;

public enum CompanyErrorType
{
    ValidationError,    // Invalid input from the client — 422
    NotFound,           // Company does not exist in the registry — 404
    ServiceUnavailable, // Brreg is down or timed out — 503
    Unexpected          // Something we did not anticipate — 500
}

public sealed record CompanyError(CompanyErrorType Type, string Message);
