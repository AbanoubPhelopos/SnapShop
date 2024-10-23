using System.ComponentModel.DataAnnotations;
namespace SnapShop.Core.ViewModels.Users
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Required]
        public string role { get; set; }
    }
}
