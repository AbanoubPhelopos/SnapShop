using SnapShop.Application.Data;

namespace SnapShop.Core.Repositories
{
    public class CategoryRepository(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        : ICategoryRepository
    {
        public List<Category?> GetCategories()
        {
            return context.Categories.ToList();
        }

        public async Task<Category?> GetCategoryAsync(int id)
        {
            return await context.Categories.FindAsync(id);
        }

        public async Task UpdateCategoryAsync(Category? category, IFormFile? image)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            
            if (image != null && image.Length > 0)
            {
                // Handle the image update
                category.Image = await HandleImageUploadAsync(image, category.Image);
            }

            context.Categories.Update(category);
            await context.SaveChangesAsync();
        }

        public async Task InsertCategoryAsync(Category category, IFormFile? image)
        {
            if (image != null && image.Length > 0)
            {
                category.Image = await HandleImageUploadAsync(image, null);
            }
            
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var category = await context.Categories.FindAsync(categoryId);
            if (category != null)
            {
                // Optionally delete the image file from the filesystem
                DeleteExistingImage(category.Image);
                
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
            }
        }

        // Helper method to handle image uploads
        private async Task<string> HandleImageUploadAsync(IFormFile image, string? existingImage = null)
        {
            if (image.Length > 500 * 1024) // 500 KB size limit
            {
                throw new Exception("Image size exceeds 500 KB.");
            }

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var filePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Categories", fileName);

            // Ensure the directory exists
            var directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Save the image file asynchronously
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Optionally delete the existing image if it's being updated
            if (!string.IsNullOrEmpty(existingImage))
            {
                DeleteExistingImage(existingImage);
            }

            return fileName;
        }

        // Helper method to delete existing images
        private void DeleteExistingImage(string? imageName)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                var imagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Categories", imageName);
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }
        }
    }
}