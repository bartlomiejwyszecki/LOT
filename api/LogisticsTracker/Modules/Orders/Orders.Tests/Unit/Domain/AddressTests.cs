using Orders.Domain.ValueObjects;

namespace Orders.Tests.Unit.Domain;

public class AddressTests
{
    #region Validation - Required Fields

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrowArgumentException_WhenStreetIsInvalid(string? invalidStreet)
    {
        // Arrange & Act
        var action = () => new Address(invalidStreet!, "City", "State", "12345", "POL");

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithParameterName("street")
            .WithMessage("Street cannot be empty*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrowArgumentException_WhenCityIsInvalid(string? invalidCity)
    {
        // Arrange & Act
        var action = () => new Address("Street", invalidCity!, "State", "12345", "POL");

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithParameterName("city")
            .WithMessage("City cannot be empty*");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenCountryIsInvalid()
    {
        // Arrange & Act
        var action = () => new Address("Street", "City", "State", "12345", "InvalidCountry");

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithParameterName("countryCode")
            .WithMessage("*ISO 3166-1 alpha-3*");
    }

    #endregion

    #region Default Value Behavior

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_ShouldSetStateToEmptyString_WhenStateIsNullOrEmpty(string? state)
    {
        // Act
        var address = new Address("Street", "City", state!, "12345", "POL");

        // Assert
        address.State.Should().Be(string.Empty);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_ShouldSetPostalCodeToEmptyString_WhenPostalCodeIsNullOrEmpty(string? postalCode)
    {
        // Act
        var address = new Address("Street", "City", "State", postalCode!, "POL");

        // Assert
        address.PostalCode.Should().Be(string.Empty);
    }

    [Fact]
    public void Constructor_ShouldCreateValidAddress_WithValidCountryCode()
    {
        // Act
        var address = new Address("Kwiatowa", "Katowice", "Silesia", "40-850", "POL");

        // Assert
        address.Country.Should().Be(CountryCode.POL);
        address.Street.Should().Be("Kwiatowa");
        address.City.Should().Be("Katowice");
    }

    #endregion
}
