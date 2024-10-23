namespace SnapShop.Core.Repositories
{
    public interface ICategoryRepository
    {
        List<Category?> GetCategories();
        Task<Category?> GetCategoryAsync(int id);
        Task InsertCategoryAsync(Category category, IFormFile? image);
        Task UpdateCategoryAsync(Category category, IFormFile? image);
        Task DeleteCategoryAsync(int id);
    }
}