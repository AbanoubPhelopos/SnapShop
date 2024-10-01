namespace SnapShop.Core.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProducts();
        Product? GetProduct(int id);
        void InsertProduct(Product product, IFormFile image);
        void UpdateProduct(Product product, IFormFile image);
        void DeleteProduct(int id);
        IEnumerable<Category?> GetCategories();
        bool IsDuplicateProductName(string productName, int? productId = null);
        bool IsDuplicateBarcode(string barcode, int? productId = null);
    }
}
