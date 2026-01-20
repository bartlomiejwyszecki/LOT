namespace Orders.Domain.ValueObjects
{
    public record Address
    {
        public string Street { get; init; } = null!;
        public string City { get; init; } = null!;
        public string State { get; init; } = null!;
        public string PostalCode { get; init; } = null!;
        public string Country { get; init; } = null!;

        private Address() { }

        public Address(string street, string city, string state, string postalCode, string country)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("Street cannot be empty", nameof(street));

            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be empty", nameof(city));

            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country cannot be empty", nameof(country));

            Street = street;
            City = city;
            State = state ?? string.Empty;
            PostalCode = postalCode ?? string.Empty;
            Country = country;
        }
    }
}
