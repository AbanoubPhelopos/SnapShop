using Microsoft.EntityFrameworkCore;
using SnapShop.Application.Data;

namespace SnapShop.Core.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductRepository(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Get all products with their categories
        public IEnumerable<Product> GetProducts()
        {
            return _context.Products.Include(p => p.Category).ToList();
        }

        // Get product by ID
        public async Task<Product?> GetProductAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        // Get all categories
        public IEnumerable<Category?> GetCategories()
        {
            return _context.Categories;
        }

        // Insert a new product and handle image upload
        public async Task InsertProductAsync(Product product, IFormFile? image)
        {
            if (image != null && image.Length > 0)
            {
                product.Image = await SaveImageAsync(image);
            }

            product.Barcode = GenerateUniqueBarcode();
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        // Update a product and handle image update
        public async Task UpdateProductAsync(Product product, IFormFile? image)
        {
            var existingProduct = await _context.Products.FindAsync(product.Id);
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
            _context.Entry(existingProduct).CurrentValues.SetValues(product);
    
            // Preserve the existing barcode
            existingProduct.Barcode = product.Barcode;

            // Update the existing product with the new values
            _context.Update(existingProduct);
            await _context.SaveChangesAsync();
        }


        // Delete product and its associated image
        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                if (!string.IsNullOrEmpty(product.Image))
                {
                    DeleteExistingImage(product.Image); // Delete image from file system
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
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
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images/Products", fileName);

            // Ensure directory exists
            if (!Directory.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "Images/Products")))
            {
                Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, "Images/Products"));
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
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images/Products", imageName);
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }
    }
}
