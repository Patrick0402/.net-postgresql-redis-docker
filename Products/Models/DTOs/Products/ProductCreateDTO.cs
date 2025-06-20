namespace ProductsApi.Models.DTOs;

public class ProductCreateDto
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
}