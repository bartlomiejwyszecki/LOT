namespace Orders.Domain.ValueObjects;

public static class CountryCodeValidator
{
    public static bool TryParse(string? countryCode, out CountryCode result)
    {
        return Enum.TryParse<CountryCode>(countryCode, out result);
    }

    public static CountryCode Parse(string? countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentException(
                "Country code cannot be empty. Must be a valid ISO 3166-1 alpha-3 country code (e.g., POL, DEU, FRA).",
                nameof(countryCode));

        if (!Enum.TryParse<CountryCode>(countryCode, out var result))
            throw new ArgumentException(
                $"Country code '{countryCode}' is invalid. Must be a valid ISO 3166-1 alpha-3 country code (e.g., POL, DEU, FRA).",
                nameof(countryCode));

        return result;
    }
}
