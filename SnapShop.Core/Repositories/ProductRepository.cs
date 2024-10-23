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
        public Product GetById(int productId)
        {
            // Fetch the product from the database by ID
            return context.Products.FirstOrDefault(p => p.Id == productId);
        }
        // Get product by ID
        public async Task<Product?> GetProductAsync(int id)
        {
            return await context.Products.FindAsync(id);
        }

        // Get all categories
        public IEnumerable<Category?> GetCategories()
        {
            return context.Categories;
        }

        public async Task InsertProductAsync(Product product, IFormFile? image)
        {
            if (image != null && image.Length > 0)
            {
                product.Image = await SaveImageAsync(image);
            }

            product.Barcode = GenerateUniqueBarcode();
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product, IFormFile? image)
        {
            var existingProduct = await context.Products.FindAsync(product.Id);
            if (existingProduct == null)
                throw new Exception("Product not found.");

            // Handle image update
            if (image != null && image.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingProduct.Image))
                {
                    DeleteExistingImage(existingProduct.Image); // Remove old image
                }
                product.Image = await SaveImageAsync(image); // Save new image
            }
            else
            {
                product.Image = existingProduct.Image; // Keep the existing image if no new image is provided
            }

            // Keep the existing barcode if it is not provided
            product.Barcode = !string.IsNullOrEmpty(product.Barcode) ? product.Barcode : existingProduct.Barcode;

            // Copy updated properties to the existing product, excluding the barcode
            context.Entry(existingProduct).CurrentValues.SetValues(product);

            // Preserve the existing barcode
            existingProduct.Barcode = product.Barcode;

            // Update the existing product with the new values
            context.Update(existingProduct);
            await context.SaveChangesAsync();
        }


        // Delete product and its associated image
        public async Task DeleteProductAsync(int id)
        {
            var product = await context.Products.FindAsync(id);
            if (product != null)
            {
                if (!string.IsNullOrEmpty(product.Image))
                {
                    DeleteExistingImage(product.Image); // Delete image from file system
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
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var filePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Products", fileName);

            // Ensure directory exists
            if (!Directory.Exists(Path.Combine(webHostEnvironment.WebRootPath, "Images/Products")))
            {
                Directory.CreateDirectory(Path.Combine(webHostEnvironment.WebRootPath, "Images/Products"));
            }

            // Save the image file
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

        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            return context.Products.Where(p => p.CategoryId == categoryId).ToList();  // Fetch products for the selected category
        }
    }
}
