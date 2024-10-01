using Microsoft.EntityFrameworkCore;
using SnapShop.Application.Data;

namespace SnapShop.Core.Repositories
{
    public class ProductRepository(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        : IProductRepository
    {
        public IEnumerable<Product> GetProducts()
        {
            return context.Products.Include(p => p.Category).ToList() ?? new List<Product>();
        }

        public IEnumerable<Category?> GetCategories()
        {
            return context.Categories;
        }

        public Product? GetProduct(int id)
        {
            return context.Products.Find(id);
        }

        public void InsertProduct(Product product, IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                if (image.Length > 500 * 1024)
                {
                    throw new Exception("Image size exceeds 500 KB.");
                }
                string fileName = $"{DateTime.Now.ToString("yyMMddHHmmss")}_{Path.GetFileName(image.FileName)}";
                var filePath = Path.Combine(webHostEnvironment.WebRootPath, "Images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                product.Image = fileName;
            }

            context.Products.Add(product);
            context.SaveChanges();
        }

        public void UpdateProduct(Product product, IFormFile image)
        {
            Product? existingProduct = context.Products.Find(product.Id);

            if (image == null)
            {
                product.Image = existingProduct.Image;
            }
            else
            {
                if (image.Length > 500 * 1024)
                {
                    throw new Exception("Image size exceeds 500 KB.");
                }

                if (!string.IsNullOrEmpty(existingProduct.Image))
                {
                    string prevFilePath = Path.Combine(webHostEnvironment.WebRootPath, "images", existingProduct.Image);
                    if (File.Exists(prevFilePath))
                    {
                        File.Delete(prevFilePath);
                    }
                }

                string fileName = $"{DateTime.Now.ToString("yyMMddHHmmss")}_{Path.GetFileName(image.FileName)}";
                var filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                product.Image = fileName;
            }
            context.Entry(existingProduct).CurrentValues.SetValues(product);
            context.SaveChanges();
        }

        public void DeleteProduct(int id)
        {
            Product? product = context.Products.Find(id);
            if (product != null)
            {
                context.Products.Remove(product);
                context.SaveChanges();
            }
        }
        public bool IsDuplicateProductName(string productName, int? productId = null)
        {
            return context.Products
                .Any(p => p.Name == productName && (!productId.HasValue || p.Id != productId.Value));
        }

        public bool IsDuplicateBarcode(string barcode, int? productId = null)
        {
            return context.Products
                .Any(p => p.Barcode == barcode && (!productId.HasValue || p.Id != productId.Value));
        }
    }
}