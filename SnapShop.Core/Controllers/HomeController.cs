using Microsoft.AspNetCore.Authorization;
using SnapShop.Utility;

namespace SnapShop.Core.Controllers;

[Authorize]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.IsInRole(StaticDetails.RoleUserStorekeeper))
        {
            return RedirectToAction("Index", "StoreKeeper");
        }else if (User.IsInRole(StaticDetails.RoleUserCashier))
        {
            return RedirectToAction("Index", "Cashier");
        }
        else
        {
            return RedirectToAction("Index", "Manager");
        }
    }
}