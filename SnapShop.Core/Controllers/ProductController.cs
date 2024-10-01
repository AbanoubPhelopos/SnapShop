namespace SnapShop.Core.Controllers
{
    public class ProductController(IProductRepository productRepository) : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Categories = productRepository.GetCategories().Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            List<Product> products = productRepository.GetProducts().ToList();
            return View(products);
        }

        [HttpPost]
        public IActionResult Create([FromForm] Product product, IFormFile image)
        {
            try
            {
                // Remove the duplicate name check
                if (productRepository.IsDuplicateBarcode(product.Barcode))
                {
                    return Json(new { success = false, message = "The barcode already exists. Please choose a different barcode." });
                }
                productRepository.InsertProduct(product, image);
                return Json(new { success = true, message = "Record created successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public IActionResult Edit(int id)
        {
            Product? product = productRepository.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }
            return Json(product);
        }

        [HttpPost]
        public IActionResult Edit([FromForm] Product product, IFormFile image)
        {
            try
            {
                // Remove the duplicate name check
                if (productRepository.IsDuplicateBarcode(product.Barcode, product.Id))
                {
                    return Json(new { success = false, message = "The barcode already exists. Please choose a different barcode." });
                }

                productRepository.UpdateProduct(product, image);
                return Json(new { success = true, message = "Record edited successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                productRepository.DeleteProduct(id);
                TempData["Toast"] = "Toast('Success','Record deleted successfully', 'success')";
            }
            catch (Exception ex)
            {
                TempData["Toast"] = $"Toast('Error','{ex.Message}', 'error')";
            }

            return RedirectToAction("Index");
        }
    }
}
