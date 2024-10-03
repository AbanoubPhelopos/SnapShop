using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapShop.Core.Models;
using SnapShop.Utility;

namespace SnapShop.Core.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (User.IsInRole(StaticDetails.RoleUserStorekeeper))
        {
            return RedirectToAction("Index", "Category");
        }else if (User.IsInRole(StaticDetails.RoleUserCashier))
        {
            return RedirectToAction("Index", "Cashier");
        }
        else
        {
            return RedirectToAction("Index", "Manager");
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}