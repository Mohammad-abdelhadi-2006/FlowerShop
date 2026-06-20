using FlowerShop.API.DTOs;

namespace FlowerShop.API.Services;

// Carries either the created order or a human-readable reason for rejection.
public record CreateOrderResult(OrderDto? Order, string? Error)
{
    public bool Success => Order is not null;

    public static CreateOrderResult Ok(OrderDto order) => new(order, null);
    public static CreateOrderResult Fail(string error) => new(null, error);
}

public interface IOrderService
{
    Task<List<OrderDto>> GetAllAsync();
    Task<OrderDto?> GetByIdAsync(int id);
    Task<CreateOrderResult> CreateAsync(CreateOrderDto dto);
    Task<bool> DeleteAsync(int id);
}
