using Microsoft.EntityFrameworkCore;
using SnapShop.Application.Data;
using SnapShop.Application.Models;

namespace SnapShop.Core.Repositories
{
    public class CashierServices(ApplicationDbContext context) : ICashierServices
    {
        public IEnumerable<Product> GetProducts(int id)
        {
            return context.Products.Include(p => p.Category).Where(p=>p.CategoryId==id).ToList();
        }

        public Product? GetProduct(int id)
        {
            var product = context.Products
                .Where(p => p.Id == id)
                .Include(p => p.Category)
                .FirstOrDefault();

            return product;
        }

        public List<Category?> GetCategories(string? query = null)
        {
            var q = context.Categories.AsNoTracking();

            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToLower();
                q = q.Where(c => c.Name.ToLower().Contains(query));
            }
            
            return q.ToList();
        }

        public Category? GetCategory(int id)
        {
            var category = context.Categories
                .FirstOrDefault(c => c.Id == id);

            return category;
        }
    }
}