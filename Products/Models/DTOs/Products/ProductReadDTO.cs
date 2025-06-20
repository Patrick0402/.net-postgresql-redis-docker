namespace ProductsApi.Models.DTOs;

public class ProductReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public DateTime ServerTime { get; set; } = DateTime.Now;
}
