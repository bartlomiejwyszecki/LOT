namespace Order.Domain.ValueObjects
{
    public record Address
    {
        public string Street { get; init; }
        public string City { get; init; }
        public string State { get; init; }
        public string PostalCode { get; init; }
        public string Country { get; init; }

        // Private parameterless constructor for EF Core
        private Address() { }

        public Address(string street, string city, string state, string postalCode, string country)
        {
            // Validation is ABSOLUTELY NECESSARY in Value Objects!
            // This ensures the object is ALWAYS in a valid state
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
