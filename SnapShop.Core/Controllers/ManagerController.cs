using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SnapShop.Application.Data;
using SnapShop.Core.ViewModels.Users;
using SnapShop.Utility;

namespace SnapShop.Core.Controllers;

[Authorize(Roles = StaticDetails.RoleUserManager)]
public class ManagerController(ApplicationDbContext context,UserManager<IdentityUser> userManager,ILogger<ManagerController> _logger, IProductRepository productRepository, RoleManager<IdentityRole> roleManager) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult GetCategories()
    {
        var categories = context.Categories.ToList();
        return View(categories);
    }

    public async Task<IActionResult> GetEmployees()
    {
        var allUsers = userManager.Users.ToList();
        var userVM = new List<UserViewModel>();

        foreach (var user in allUsers)
        {
            var roles = await userManager.GetRolesAsync(user);

            userVM.Add(new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Role = string.Join(", ", roles) // Join roles into a comma-separated string
            });
        }

        return View(userVM); // Return the view with the list of user view models
    }



    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        // Load categories asynchronously
        ViewBag.Categories = (await Task.FromResult(productRepository.GetCategories()))
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

        // Get products asynchronously
        List<Product> products = productRepository.GetProducts().ToList();
        return View(products);
    }
    [HttpPost]
    public async Task<IActionResult> EditUser(string userId, string email, string role, string password, string confirmPassword)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Json(new { success = false, message = "User  not found." });
        }

        user.Email = email;

        // Update the user's role
        var roles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, roles);
        await userManager.AddToRoleAsync(user, role);

        // Update the user's password
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        await userManager.ResetPasswordAsync(user, token, password);

        return Json(new { success = true, message = "User  updated successfully." });
    }


    [HttpPost]
    public async Task<IActionResult> LockUser(string userId)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            user.LockoutEnabled = true;
            await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddMinutes(30)); // Lock for 30 minutes
            var updateResult = await userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return Json(new { success = false, message = "Failed to lock user." });
            }

            return Json(new { success = true, message = "User has been locked." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error locking out user {UserId}: {Message}", userId, ex.Message);
            return Json(new { success = false, message = "An error occurred while locking out the user." });
        }
    }
    [HttpPost]
    public async Task<IActionResult> UnlockUser(string userId)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            // Ensure that lockout is enabled for the user before unlocking
            user.LockoutEnabled = true;

            // Set the lockout end date to null to unlock the user
            await userManager.SetLockoutEndDateAsync(user, null);

            // Ensure that the user is updated
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return Json(new { success = false, message = "Failed to update user." });
            }

            return Json(new { success = true, message = "User has been unlocked." });
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "Error unlocking user {UserId}: {Message}", userId, ex.Message);

            // Return a JSON response with an error message
            return Json(new { success = false, message = "An error occurred while unlocking the user. Please try again." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> RemoveEmployee(string userId)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "User  not found." });
            }

            await userManager.DeleteAsync(user);
            return Json(new { success = true, message = "Employee deleted successfully!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "An error occurred: " + ex.Message });
        }
    }
}
