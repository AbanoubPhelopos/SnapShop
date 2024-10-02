using System.ComponentModel.DataAnnotations;

namespace SnapShop.Core.ViewModels.Categories
{
    public record CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; init; }

        [Required(ErrorMessage = "Image is required")]
        public IFormFile Image { get; init; }
    }

    public record EditCategoryViewModel
    {
        [Required]
        public int Id { get; init; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; init; }

        public IFormFile Image { get; init; }
    }
}