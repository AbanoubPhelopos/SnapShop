using Microsoft.AspNetCore.Authorization;
using SnapShop.Utility;

namespace SnapShop.Core.Controllers;

[Authorize(Roles = StaticDetails.RoleUserStorekeeper)]
public class StoreKeeperController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Category");
    }
}