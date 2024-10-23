using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnapShop.Application.Models
{
    public class Product
    {
        [Key]
        [Column("ProductId")]
        public int Id { get; set; }

        [Required]
        [Column("ProductName")]
        [MaxLength(100)] 
        public string Name { get; set; } = string.Empty;

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [Required]
        [Column("ProductImage")]
        public string Image { get; set; } = string.Empty;

        [Column("ProductQuantity")]
        [Range(0, 1000)]
        public int Quantity { get; set; }

        [Column("ProductDescription")]
        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Column("ProductPrice")]
        [Range(0.0, 10000.0)]
        public decimal Price { get; set; }

        /*[Required]
        [Column("ProductBrand")]
        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;*/

        [Required]
        [Column("ProductBarcode")]
        [MaxLength(50)]
        public string Barcode { get; set; } = string.Empty;

        // Navigation property
        public Category? Category { get; set; }
    }
}