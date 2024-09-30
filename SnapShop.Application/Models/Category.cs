using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnapShop.Application.Models
{
    public class Category
    {
        [Key]
        [Column("CategoryId")] 
        public int Id { get; set; }

        [Required]
        [MaxLength(100)] 
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("CategoryImage")] 
        public string Image { get; set; } = string.Empty;
    }
}