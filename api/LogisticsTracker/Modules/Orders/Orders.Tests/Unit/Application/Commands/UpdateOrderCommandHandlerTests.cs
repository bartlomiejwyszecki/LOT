using Orders.Application.Commands.UpdateOrderStatus;
using Orders.Application.DTOs;
using Orders.Application.Interfaces;
using Orders.Domain.Entities;
using Orders.Domain.ValueObjects;

namespace Orders.Tests.Unit.Application.Commands
{
    public class UpdateOrderCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _mockRepository;
        private readonly UpdateOrderStatusCommandHandler _handler;

        public UpdateOrderCommandHandlerTests()
        {
            _mockRepository = new Mock<IOrderRepository>();
            _handler = new UpdateOrderStatusCommandHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task HandleAsync_ShouldUpdateOrderStatusAndReturnDto()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var newStatus = OrderStatus.Confirmed;
            var command = new UpdateOrderStatusCommand(orderId, newStatus);

            var address = new Address("Kwiatowa", "Katowice", "Silesia", "40-850", "POL");
            var order = Order.Create("ORD-001", address);

            _mockRepository
                .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _mockRepository
                .Setup(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockRepository
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(newStatus);
            result.OrderNumber.Should().Be("ORD-001");
            result.Id.Should().Be(order.Id);

            _mockRepository.Verify(
                r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()),
                Times.Once);
            _mockRepository.Verify(
                r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _mockRepository.Verify(
                r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenOrderNotFound_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var newStatus = OrderStatus.Confirmed;
            var command = new UpdateOrderStatusCommand(orderId, newStatus);

            _mockRepository
                .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            // Act & Assert
            await FluentActions
                .Invoking(() => _handler.HandleAsync(command))
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage($"Order with ID {orderId} not found.");

            _mockRepository.Verify(
                r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()),
                Times.Once);

            _mockRepository.Verify(
                r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenInvalidStatusTransition_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var invalidStatus = OrderStatus.Shipped;
            var command = new UpdateOrderStatusCommand(orderId, invalidStatus);

            var address = new Address("Przykladowa", "Warsaw", "Mazovia", "00-001", "POL");
            var order = Order.Create("ORD-002", address);

            _mockRepository
                .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            // Act & Assert
            await FluentActions
                .Invoking(() => _handler.HandleAsync(command))
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*Invalid status transition*");

            _mockRepository.Verify(
                r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()),
                Times.Once);

            _mockRepository.Verify(
                r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _mockRepository.Verify(
                r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenRepositoryThrows_ShouldPropagateException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var newStatus = OrderStatus.Confirmed;
            var command = new UpdateOrderStatusCommand(orderId, newStatus);

            var address = new Address("Glowna", "Cracow", "Małopolskie", "31-999", "POL");
            var order = Order.Create("ORD-003", address);

            var repositoryException = new InvalidOperationException("Database connection failed");

            _mockRepository
                .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _mockRepository
                .Setup(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(repositoryException);

            // Act & Assert
            await FluentActions
                .Invoking(() => _handler.HandleAsync(command))
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Database connection failed");

            _mockRepository.Verify(
                r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()),
                Times.Once);

            _mockRepository.Verify(
                r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
                Times.Once);

            _mockRepository.Verify(
                r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
