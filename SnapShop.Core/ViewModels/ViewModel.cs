using System.ComponentModel.DataAnnotations;

namespace SnapShop.Core.ViewModels
{
    public class UserViewModel
    {
        [Required]
        public string Email { get; set; }= string.Empty;
        public string UserRole { get; set; } = string.Empty;
    }
}
