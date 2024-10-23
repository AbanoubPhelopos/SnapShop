using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SnapShop.Application.Data;
using SnapShop.Application.ViewModel;
using SnapShop.Core.ViewModels.Users;
using SnapShop.Utility;

namespace SnapShop.Core.Controllers
{
    [Authorize(Roles = StaticDetails.RoleUserManager)]
    public class ManagerController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        ILogger<ManagerController> logger,
        IProductRepository productRepository,
        RoleManager<IdentityRole> roleManager)
        : Controller
    {

        public IActionResult Index()
        {
            return RedirectToAction("GetEmployees");
        }

        public IActionResult GetCategories()
        {
            var categories = context.Categories.ToList();
            return View(categories);
        }

        public async Task<IActionResult> GetEmployees()
        {
            var allUsers = await userManager.Users.ToListAsync();
            var userVM = new List<UserVM>();

            foreach (var user in allUsers)
            {
                var roles = await userManager.GetRolesAsync(user);
                var isLocked = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow;

                userVM.Add(new UserVM
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = string.Join(", ", roles), 
                    IsLocked = isLocked 
                });
            }

            return View(userVM);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            ViewBag.Categories = (await Task.FromResult(productRepository.GetCategories()))
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            List<Product> products = productRepository.GetProducts().ToList();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            IdentityUser _user = await userManager.FindByIdAsync(id);
            if (_user == null)
            {
                return NotFound();
            }

            var Roles = await userManager.GetRolesAsync(_user);
            var _role = Roles.FirstOrDefault();

            return Json(new { success = true, user = _user, role = _role });
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] EditUserViewModel Model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IdentityUser user = await userManager.FindByIdAsync(Model.Id);
                    if (user != null)
                    {
                        var isUserInRole = await userManager.IsInRoleAsync(user, Model.role);
                        if (!isUserInRole)
                        {
                            var roles = await userManager.GetRolesAsync(user);
                            await userManager.RemoveFromRolesAsync(user, roles);
                            var result = await userManager.AddToRoleAsync(user, Model.role);
                            if (!result.Succeeded)
                            {
                                return BadRequest("Failed to add user to role: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                            }
                            return Json(new { success = true, message = "Record edited successfully!" });
                        }

                        return BadRequest(new { message = "User is already in this role." });
                    }
                    return NotFound();
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = "Validation failed.", errors });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLockUser(string userId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                bool isCurrentlyLocked = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow;

                if (isCurrentlyLocked)
                {
                    await userManager.SetLockoutEndDateAsync(user, null);
                    user.LockoutEnabled = false;

                    var updateResult = await userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        return Json(new { success = false, message = "Failed to unlock the user." });
                    }

                    return Json(new { success = true, message = "User has been unlocked.", isLocked = false });
                }
                else
                {
                    user.LockoutEnabled = true;
                    await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddMinutes(30));

                    var updateResult = await userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        return Json(new { success = false, message = "Failed to lock the user." });
                    }

                    return Json(new { success = true, message = "User has been locked.", isLocked = true });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error toggling lock for user {UserId}: {Message}", userId, ex.Message);
                return Json(new { success = false, message = "An error occurred while toggling the lock status." });
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
                    return Json(new { success = false, message = "User not found." });
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
}
