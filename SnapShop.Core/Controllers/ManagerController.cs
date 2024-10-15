using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapShop.Application.Data;
using SnapShop.Utility;

namespace SnapShop.Core.Controllers;

[Authorize(Roles = StaticDetails.RoleUserManager)]
public class ManagerController(ApplicationDbContext context) : Controller
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
}