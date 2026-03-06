using System.Text.Json.Serialization;

namespace Infrastructure.Companies;

// Only map the fields we actually use. Extra fields in the Brreg response are ignored.
// All fields are nullable to avoid JsonException when the API omits optional properties.
internal sealed class BrregResponseDto
{
    [JsonPropertyName("organisasjonsnummer")]
    public string? Organisasjonsnummer { get; init; }

    [JsonPropertyName("navn")]
    public string? Navn { get; init; }

    [JsonPropertyName("organisasjonsform")]
    public BrregOrganizationFormDto? Organisasjonsform { get; init; }

    [JsonPropertyName("maalform")]
    public string? Maalform { get; init; }
}

internal sealed class BrregOrganizationFormDto
{
    [JsonPropertyName("kode")]
    public string? Kode { get; init; }
}
