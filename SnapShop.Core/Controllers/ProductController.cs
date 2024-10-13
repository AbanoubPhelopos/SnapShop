using Microsoft.AspNetCore.Authorization;
using SnapShop.Core.ViewModels.Products;
using SnapShop.Utility;

namespace SnapShop.Core.Controllers
{
    [Authorize(Roles = StaticDetails.RoleUserStorekeeper)]
    public class ProductController(IProductRepository productRepository) : Controller
    { 
        public async Task<IActionResult> Index()
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
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductsVM.Create modal)
        {
            try
            {
                // Insert the product asynchronously
                await productRepository.InsertProductAsync(new()
                {
                    Name = modal.Name,
                    Description = modal.Description,
                    CategoryId = modal.CategoryId,
                    Quantity = modal.Quantity,
                    Price = modal.Price,
                }, modal.Image);
                return Json(new { success = true, message = "Record created successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            // Fetch product asynchronously
            Product? product = await productRepository.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Json(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] Product product, IFormFile image)
        {
            try
            {
                // Update the product asynchronously
                await productRepository.UpdateProductAsync(product, image);
                return Json(new { success = true, message = "Record edited successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await productRepository.DeleteProductAsync(id);
                return Json(new { success = true, message = "Record deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
    }
}
