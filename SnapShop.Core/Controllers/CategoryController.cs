namespace SnapShop.Core.Controllers
{
    public class CategoryController(ICategoryRepository categoryRepository) : Controller
    {
        public IActionResult Index()
        {
            List<Category> categories = categoryRepository.GetCategories().ToList();
            return View(categories);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    categoryRepository.InsertCategory(category);
                    return Json(new { success = true, message = "Record created successfully!" });
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = "Validation failed.", errors = errors });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Edit(int id)
        {
            Category? category = categoryRepository.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }
            return Json(category);
        }

        [HttpPost]
        public IActionResult Edit([FromBody] Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    categoryRepository.UpdateCategory(category);
                    return Json(new { success = true, message = "Record edited successfully!" });
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = "Validation failed.", errors = errors });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }
        
        public IActionResult Delete(int id)
        {
            try
            {
                categoryRepository.DeleteCategory(id);
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