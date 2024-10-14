namespace SnapShop.Core.Repositories;

public interface ICashierServices
{
    IEnumerable<Product> GetProducts(int id);
    Product GetProduct(int id);
    List<Category?> GetCategories(string? query = null);

    List<Product> GetAllProducts();
}