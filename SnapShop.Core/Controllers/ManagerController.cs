using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SnapShop.Application.Data;
using SnapShop.Utility;

namespace SnapShop.Core.Controllers;

[Authorize(Roles = StaticDetails.RoleUserManager)]
public class ManagerController(ApplicationDbContext context,UserManager<IdentityUser> userManager) : Controller
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

    public IActionResult GetEmployees()
    {
        var users = userManager.Users.ToList();
        return View(users);
    }
}