using Order.Domain.Rules;
using Order.Domain.ValueObjects;

namespace Order.Domain.Entities
{
    public class Order : Entity
    {
        public string OrderNumber { get; private set; }
        public OrderStatus Status { get; private set; }
        public Address ShippingAddress { get; private set; }
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
