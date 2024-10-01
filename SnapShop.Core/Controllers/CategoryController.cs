namespace SnapShop.Core.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            var categories = categoryRepository.GetCategories().ToList();
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Category category, IFormFile? image)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await categoryRepository.InsertCategoryAsync(category, image);
                    return Json(new { success = true, message = "Record created successfully!" });
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = "Validation failed.", errors });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await Task.FromResult(categoryRepository.GetCategoryAsync(id));
            if (category == null)
            {
                return NotFound();
            }
            return Json(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] Category category, IFormFile? image)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await categoryRepository.UpdateCategoryAsync(category, image);
                    return Json(new { success = true, message = "Record edited successfully!" });
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = "Validation failed.", errors });
                }
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
                await categoryRepository.DeleteCategoryAsync(id);
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