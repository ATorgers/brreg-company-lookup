using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Companies;
using CSharpFunctionalExtensions;
using Domain.Companies;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Companies;

internal sealed class BrregCompanyService(
    HttpClient httpClient,
    ILogger<BrregCompanyService> logger) : ICompanyService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<Result<Company, CompanyError>> GetCompanyAsync(
        OrganizationNumber organizationNumber,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync(
                $"enheter/{organizationNumber.Value}",
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogWarning(
                    "Company not found for organization number {OrganizationNumber}",
                    organizationNumber.Value);

                return Result.Failure<Company, CompanyError>(new CompanyError(
                    CompanyErrorType.NotFound,
                    $"No company found with organization number {organizationNumber.Value}."));
            }

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError(
                    "Unexpected status code {StatusCode} when looking up organization number {OrganizationNumber}",
                    (int)response.StatusCode,
                    organizationNumber.Value);

                return Result.Failure<Company, CompanyError>(new CompanyError(
                    CompanyErrorType.Unexpected,
                    $"Unexpected error retrieving company data. HTTP status: {(int)response.StatusCode}."));
            }

            var dto = await response.Content
                .ReadFromJsonAsync<BrregResponseDto>(JsonOptions, cancellationToken);

            if (dto is null)
            {
                return Result.Failure<Company, CompanyError>(new CompanyError(
                    CompanyErrorType.Unexpected,
                    "Failed to deserialize company data from Brreg."));
            }

            var company = new Company(
                OrganizationNumber: dto.Organisasjonsnummer,
                OrganizationName: dto.Navn,
                CompanyType: dto.Organisasjonsform?.Kode ?? string.Empty,
                LanguageForm: dto.Maalform ?? string.Empty);

            logger.LogInformation(
                "Successfully retrieved company {CompanyName} ({OrganizationNumber})",
                company.OrganizationName,
                organizationNumber.Value);

            return Result.Success<Company, CompanyError>(company);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogError(
                ex,
                "Request timed out for organization number {OrganizationNumber}",
                organizationNumber.Value);

            return Result.Failure<Company, CompanyError>(new CompanyError(
                CompanyErrorType.ServiceUnavailable,
                "The request to Brreg timed out. Please try again."));
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(
                ex,
                "HTTP error when looking up organization number {OrganizationNumber}",
                organizationNumber.Value);

            return Result.Failure<Company, CompanyError>(new CompanyError(
                CompanyErrorType.ServiceUnavailable,
                "The Brreg service is currently unavailable. Please try again later."));
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error when looking up organization number {OrganizationNumber}",
                organizationNumber.Value);

            return Result.Failure<Company, CompanyError>(new CompanyError(
                CompanyErrorType.Unexpected,
                "An unexpected error occurred while retrieving company data."));
        }
    }
}
