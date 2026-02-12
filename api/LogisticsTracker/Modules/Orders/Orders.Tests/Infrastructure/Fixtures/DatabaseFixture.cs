using Microsoft.EntityFrameworkCore;
using Orders.Infrastructure.Persistence;

namespace Orders.Tests.Infrastructure.Fixtures;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly DbContextOptions<OrdersDbContext> _options;
    private OrdersDbContext? _context;

    public DatabaseFixture()
    {
        _options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;
    }

    public OrdersDbContext DbContext
    {
        get => _context ?? throw new InvalidOperationException("DbContext not initialized. Call InitializeAsync first.");
    }

    public async Task InitializeAsync()
    {
        _context = new OrdersDbContext(_options);

        await _context.Database.OpenConnectionAsync();
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        if (_context is not null)
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }
    }
}