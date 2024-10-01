namespace SnapShop.Core.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProducts();
        Task<Product?> GetProductAsync(int id);
        Task InsertProductAsync(Product product, IFormFile image);
        Task UpdateProductAsync(Product product, IFormFile image);
        Task DeleteProductAsync(int id);
        IEnumerable<Category?> GetCategories();
    }
}