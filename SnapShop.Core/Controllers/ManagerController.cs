using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapShop.Utility;

namespace SnapShop.Core.Controllers;

[Authorize(Roles = StaticDetails.RoleUserManager)]
public class ManagerController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}