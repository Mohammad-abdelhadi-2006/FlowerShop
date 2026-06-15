namespace FlowerShop.API.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }

    // Foreign key + navigation to the user who placed the order
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // Navigation: an order has many items
    public List<OrderItem> OrderItems { get; set; } = new();
}
