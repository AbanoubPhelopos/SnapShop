using SnapShop.Application.Data;

namespace SnapShop.Core.Repositories
{
    public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
    {
        public IEnumerable<Category> GetCategories()
        {
            return context.Categories.ToList();
        }

        public Category? GetCategory(int id)
        {
            return context.Categories.Find(id);
        }
        public void UpdateCategory(Category? category)
        {
            context.Categories.Update(category);
            context.SaveChanges();
        }
        public void UpdateCategory(int id, Category? category)
        {
            context.Categories.Update(category);
            context.SaveChanges();
        }

        public void InsertCategory(Category? category)
        {
                context.Categories.Add(category);
                context.SaveChanges();
        }

        public void DeleteCategory(int categoryId)
        {
            var category = context.Categories.Find(categoryId);
            if (category != null)
            {
                context.Categories.Remove(category);
                context.SaveChanges();
            }
        }
    }
}