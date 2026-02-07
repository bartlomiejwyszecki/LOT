namespace Orders.Domain.ValueObjects;

public record Address
{
    public string Street { get; init; } = null!;
    public string City { get; init; } = null!;
    public string State { get; init; } = null!;
    public string PostalCode { get; init; } = null!;
    public CountryCode Country { get; init; }

    private Address() { }

    public Address(string street, string city, string state, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty", nameof(street));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));

        var countryCode = CountryCodeValidator.Parse(country);

        Street = street;
        City = city;
        State = state ?? string.Empty;
        PostalCode = postalCode ?? string.Empty;
        Country = countryCode;
    }
}
