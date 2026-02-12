using Orders.Domain.Entities;
using Orders.Domain.ValueObjects;

namespace Orders.Tests.Infrastructure.Fixtures;

public class TestDataBuilder
{
    private string _orderNumber = "ORD-001";
    private string _street = "Kwiatowa 15";
    private string _city = "Katowice";
    private string _state = "Śląskie";
    private string _postalCode = "40-850";
    private string _countryCode = "POL";
    private OrderStatus _status = OrderStatus.Pending;

    public TestDataBuilder WithOrderNumber(string orderNumber)
    {
        _orderNumber = orderNumber;
        return this;
    }

    public TestDataBuilder WithStreet(string street)
    {
        _street = street;
        return this;
    }

    public TestDataBuilder WithCity(string city)
    {
        _city = city;
        return this;
    }

    public TestDataBuilder WithState(string state)
    {
        _state = state;
        return this;
    }

    public TestDataBuilder WithPostalCode(string postalCode)
    {
        _postalCode = postalCode;
        return this;
    }

    public TestDataBuilder WithCountryCode(string countryCode)
    {
        _countryCode = countryCode;
        return this;
    }

    public TestDataBuilder WithStatus(OrderStatus status)
    {
        _status = status;
        return this;
    }

    public Order Build()
    {
        var address = new Address(_street, _city, _state, _postalCode, _countryCode);
        var order = Order.Create(_orderNumber, address);

        if (_status != OrderStatus.Pending)
        {
            order.UpdateStatus(_status);
        }

        return order;
    }

    public Address BuildAddress()
    {
        return new Address(_street, _city, _state, _postalCode, _countryCode);
    }
}