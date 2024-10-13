namespace SnapShop.Core.Repositories
{
    public interface IOrderRepository
    {
        void AddOrder(Order order); // Add a new order
        Order GetOrderById(int orderId); // Retrieve an order by ID
        List<Order> GetAllOrders(); // Retrieve all orders
        void UpdateOrder(Order order); // Update an existing order
        void DeleteOrder(int orderId); // Delete an order by ID
    }
}
