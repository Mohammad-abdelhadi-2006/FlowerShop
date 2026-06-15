namespace FlowerShop.API.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }

    // Navigation: a product can appear in many order items
    public List<OrderItem> OrderItems { get; set; } = new();
}
