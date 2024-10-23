namespace SnapShop.Core.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProducts();
        Product GetById(int productId);
        IEnumerable<Product> GetProductsByCategory(int categoryId);
        Task<Product?> GetProductAsync(int id);
        Task InsertProductAsync(Product product, IFormFile image);
        Task UpdateProductAsync(Product product, IFormFile image);
        Task DeleteProductAsync(int id);
        IEnumerable<Category?> GetCategories();
    }
}