using SnapShop.Application.Data;

namespace SnapShop.Core.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryRepository(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Get list of all categories
        public List<Category?> GetCategories()
        {
            return _context.Categories.ToList();
        }

        // Get category by ID
        public async Task<Category?> GetCategoryAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        // Update category and handle image update
        public async Task UpdateCategoryAsync(Category? category, IFormFile? formFile)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));

            string wwwRootPath = _webHostEnvironment.WebRootPath;

            // Handle image update logic
            if (formFile != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
                string categoryImagePath = Path.Combine(wwwRootPath, @"Images\Categories");

                // Remove the existing image if it exists
                if (!string.IsNullOrEmpty(category.Image))
                {
                    var oldImagePath = Path.Combine(wwwRootPath, category.Image.Trim('\\'));
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }

                // Save the new image
                using (var fileStream = new FileStream(Path.Combine(categoryImagePath, fileName), FileMode.Create))
                {
                    await formFile.CopyToAsync(fileStream);
                }

                // Update category with new image URL
                category.Image = @"Images\Categories\" + fileName;
            }

            // Update category in the database
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        // Insert new category and handle image upload
        public async Task InsertCategoryAsync(Category category, IFormFile? formFile)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            if (formFile != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
                string categoryImagePath = Path.Combine(wwwRootPath, @"Images\Categories");

                // Save the new image
                using (var fileStream = new FileStream(Path.Combine(categoryImagePath, fileName), FileMode.Create))
                {
                    await formFile.CopyToAsync(fileStream);
                }

                // Set image URL in category object
                category.Image = @"Images\Categories\" + fileName;
            }

            // Add category to the database
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        // Delete category and remove the associated image
        public async Task DeleteCategoryAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category != null)
            {
                // Delete the image file if it exists
                if (!string.IsNullOrEmpty(category.Image))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, category.Image.Trim('\\'));
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }

                // Remove category from the database
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
