using FlowerShop.API.Data;
using FlowerShop.API.DTOs;
using FlowerShop.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.API.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderDto>> GetAllAsync()
    {
        // Include items (and their product) or the lines come back empty.
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .ToListAsync();

        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order is null ? null : MapToDto(order);
    }

    public async Task<CreateOrderResult> CreateAsync(CreateOrderDto dto)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
        if (!userExists)
            return CreateOrderResult.Fail($"User {dto.UserId} does not exist.");

        var order = new Order
        {
            UserId = dto.UserId,
            OrderDate = DateTime.UtcNow,
            OrderItems = new List<OrderItem>()
        };

        decimal total = 0m;

        foreach (var item in dto.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product is null)
                return CreateOrderResult.Fail($"Product {item.ProductId} does not exist.");

            if (product.StockQuantity < item.Quantity)
                return CreateOrderResult.Fail(
                    $"Insufficient stock for '{product.Name}': requested {item.Quantity}, available {product.StockQuantity}.");

            // Snapshot the price at order time — not a live reference to the product.
            var unitPrice = product.Price;
            var lineTotal = unitPrice * item.Quantity;
            total += lineTotal;

            order.OrderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = unitPrice
            });

            // Decrease stock.
            product.StockQuantity -= item.Quantity;
        }

        order.TotalAmount = total;

        _context.Orders.Add(order);
        // One save commits the order, its items, and the stock changes together.
        await _context.SaveChangesAsync();

        // Reload product names for the response without another round trip per item.
        var created = await GetByIdAsync(order.Id);
        return CreateOrderResult.Ok(created!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
            return false;

        _context.OrderItems.RemoveRange(order.OrderItems);
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }

    private static OrderDto MapToDto(Order order) => new()
    {
        Id = order.Id,
        UserId = order.UserId,
        Total = order.TotalAmount,
        CreatedAt = order.OrderDate,
        Items = order.OrderItems.Select(oi => new OrderItemDto
        {
            ProductId = oi.ProductId,
            ProductName = oi.Product?.Name ?? string.Empty,
            Quantity = oi.Quantity,
            UnitPrice = oi.UnitPrice
        }).ToList()
    };
}
