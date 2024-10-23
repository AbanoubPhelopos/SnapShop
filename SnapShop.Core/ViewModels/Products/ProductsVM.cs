using System.ComponentModel.DataAnnotations;

namespace SnapShop.Core.ViewModels.Products;

public static class ProductsVM
{
    public record Create
    {
 

        
        [Required]
        [MaxLength(100)] 
        public string Name { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }


        [Range(0, 1000)]
        public int Quantity { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0.0, 10000.0)]
        public decimal Price { get; set; }
        
        [Required] public IFormFile Image { get; set; }
    }
}