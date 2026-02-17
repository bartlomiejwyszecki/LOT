using Orders.Domain.Rules;
using Orders.Domain.ValueObjects;

namespace Orders.Domain.Entities
{
    public class Order : Entity
    {
        public string OrderNumber { get; private set; } = null!;
        public OrderStatus Status { get; private set; }
        public Address ShippingAddress { get; private set; } = null!;
        public DateTime OrderDate { get; private set; }

        private Order() { }

        private Order(string orderNumber, Address shippingAddress)
        {
            OrderNumber = orderNumber;
            ShippingAddress = shippingAddress;
            Status = OrderStatus.Pending;
            OrderDate = DateTime.UtcNow;
        }

        public static Order Create(string orderNumber, Address shippingAddress)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
                throw new ArgumentException("Order number cannot be empty or whitespace.", nameof(orderNumber));

            return new Order(orderNumber, shippingAddress);
        }

        public void UpdateStatus(OrderStatus newStatus)
        {
            OrderStatusTransitionRule.ValidateTransition(Status, newStatus);
            
            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
