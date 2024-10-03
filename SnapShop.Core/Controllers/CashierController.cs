using Microsoft.AspNetCore.Authorization;
using SnapShop.Utility;

namespace SnapShop.Core.Controllers;

[Authorize(Roles = StaticDetails.RoleUserCashier)]
public class CashierController(ICashierServices cashierServices) : Controller
{

    [HttpGet]
    public IActionResult Index()
    {
        var categories = cashierServices.GetCategories().ToList();
        return View(categories);
    }
    
    [HttpGet]
    public IActionResult GetProducts(int id)
    {
        var products = cashierServices.GetProducts(id).ToList();
        return View(products);
    }
    [HttpGet]
    public IActionResult GetProduct(int id)
    {
        var product = cashierServices.GetProduct(id);
        return View(product);
    }
}