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
        var action = () => new Address(invalidStreet!, "City", "State", "12345", "Country");

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
        var action = () => new Address("Street", invalidCity!, "State", "12345", "Country");

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithParameterName("city")
            .WithMessage("City cannot be empty*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrowArgumentException_WhenCountryIsInvalid(string? invalidCountry)
    {
        // Arrange & Act
        var action = () => new Address("Street", "City", "State", "12345", invalidCountry!);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithParameterName("country")
            .WithMessage("Country cannot be empty*");
    }

    #endregion

    #region Default Value Behavior

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_ShouldSetStateToEmptyString_WhenStateIsNullOrEmpty(string? state)
    {
        // Act
        var address = new Address("Street", "City", state!, "12345", "Country");

        // Assert
        address.State.Should().Be(string.Empty);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_ShouldSetPostalCodeToEmptyString_WhenPostalCodeIsNullOrEmpty(string? postalCode)
    {
        // Act
        var address = new Address("Street", "City", "State", postalCode!, "Country");

        // Assert
        address.PostalCode.Should().Be(string.Empty);
    }

    #endregion
}
