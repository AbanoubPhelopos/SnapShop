using Microsoft.EntityFrameworkCore;
using SnapShop.Application.Data;

namespace SnapShop.Core.Repositories
{
    public class ProductRepository(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        : IProductRepository
    {
        public IEnumerable<Product> GetProducts()
        {
            return context.Products.Include(p => p.Category).ToList();
        }

        public async Task<Product?> GetProductAsync(int id)
        {
            return await context.Products.FindAsync(id);
        }

        public IEnumerable<Category?> GetCategories()
        {
            return context.Categories;
        }

        public async Task InsertProductAsync(Product product, IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                product.Image = await SaveImageAsync(image);
            }

            product.Barcode = GenerateUniqueBarcode();
            context.Products.Add(product);
            await context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product, IFormFile image)
        {
            var existingProduct = await context.Products.FindAsync(product.Id);
            if (existingProduct == null)
                throw new Exception("Product not found.");

            if (image != null && image.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingProduct.Image))
                {
                    DeleteExistingImage(existingProduct.Image);
                }

                product.Image = await SaveImageAsync(image);
            }
            else
            {
                product.Image = existingProduct.Image;
            }

            context.Entry(existingProduct).CurrentValues.SetValues(product);
            await context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await context.Products.FindAsync(id);
            if (product != null)
            {
                if (!string.IsNullOrEmpty(product.Image))
                {
                    DeleteExistingImage(product.Image);
                }

                context.Products.Remove(product);
                await context.SaveChangesAsync();
            }
        }

        // Helper method to generate a unique 6-character barcode
        private string GenerateUniqueBarcode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
        }

        // Helper method to save images to the file system
        private async Task<string> SaveImageAsync(IFormFile image)
        {
            if (image.Length > 500 * 1024) // 500 KB size limit
            {
                throw new Exception("Image size exceeds 500 KB.");
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var filePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Products", fileName);

            if (!Directory.Exists(Path.Combine(webHostEnvironment.WebRootPath, "Images/Products")))
            {
                Directory.CreateDirectory(Path.Combine(webHostEnvironment.WebRootPath, "Images/Products"));
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return fileName;
        }

        // Helper method to delete existing images
        private void DeleteExistingImage(string imageName)
        {
            var imagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Products", imageName);
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }
    }
}