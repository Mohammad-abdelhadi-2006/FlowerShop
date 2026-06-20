using System.ComponentModel.DataAnnotations;

namespace FlowerShop.API.DTOs;

public class UpdateUserDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
