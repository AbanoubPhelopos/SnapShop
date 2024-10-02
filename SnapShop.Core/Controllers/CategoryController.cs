using SnapShop.Core.ViewModels.Categories;

namespace SnapShop.Core.Controllers
{
    public class CategoryController(ICategoryRepository categoryRepository) : Controller
    {
        public IActionResult Index()
        {
            var categories = categoryRepository.GetCategories().ToList();
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateCategoryViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await categoryRepository.InsertCategoryAsync(new Category
                    {
                        Name = model.Name
                    }, model.Image);
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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await categoryRepository.GetCategoryAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Json(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] EditCategoryViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await categoryRepository.UpdateCategoryAsync(new Category
                    {
                        Id = model.Id,
                        Name = model.Name
                    }, model.Image);
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
                return Json(new { success = true, message = "Record deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
    }
}