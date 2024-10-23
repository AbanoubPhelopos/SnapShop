using SnapShop.Application.Data;

namespace SnapShop.Core.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrderRepository(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order); // Add the order to the context
            _context.SaveChanges(); // Save changes to the database
        }

        public Order GetOrderById(int orderId)
        {
            return _context.Orders.Find(orderId); // Retrieve the order by ID
        }

        public List<Order> GetAllOrders()
        {
            return _context.Orders.ToList(); // Retrieve all orders as a list
        }

        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order); // Update the existing order
            _context.SaveChanges(); // Save changes to the database
        }

        public void DeleteOrder(int orderId)
        {
            var order = GetOrderById(orderId); // Find the order
            if (order != null)
            {
                _context.Orders.Remove(order); // Remove the order from the context
                _context.SaveChanges(); // Save changes to the database
            }
        }
    }
}
