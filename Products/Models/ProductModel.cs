using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductsApi.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "O nome do produto deve ter no máximo 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(8,2)")]
        [Range(0, 999999.99, ErrorMessage = "O preço deve ser entre 0 e 999.999,99.")]
        public decimal Price { get; set; }
    }
}
