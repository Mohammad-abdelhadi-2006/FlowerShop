using System.ComponentModel.DataAnnotations;

namespace FlowerShop.API.DTOs;

public class CreateOrderDto
{
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreateOrderItemDto> Items { get; set; } = new();
}
