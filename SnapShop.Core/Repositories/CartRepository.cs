using Microsoft.EntityFrameworkCore;
using SnapShop.Application.Data;

namespace SnapShop.Core.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Constructor
        public CartRepository(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Add item to the cart
        public void AddToCart(int productId, int quantity)
        {
            var cartItem = new CartItem
            {
                ProductId = productId,
                Quantity = quantity,
            };
            _context.CartItems.Add(cartItem);
            _context.SaveChanges();
        }

        // Retrieve all cart items
        public List<CartItem> GetCartItems()
        {
            return _context.CartItems.Include(c => c.Product).ToList();
        }
        public CartItem GetCartItemById(int cartItemId)
        {
            return _context.CartItems .Include(c => c.Product).FirstOrDefault(c => c.CartItemId == cartItemId);
        }

        // Update item in the cart
        public void UpdateCartItem(int cartItemId, int quantity)
        {
            var cartItem = GetCartItemById(cartItemId);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                _context.SaveChanges();
            }
        }

        // Remove item from the cart
        public void RemoveFromCart(int cartItemId)
        {
            var cartItem = _context.CartItems.FirstOrDefault(c => c.CartItemId == cartItemId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }
        }
        public CartItem GetCartItem(int productId)
        {
            return _context.CartItems.FirstOrDefault(c => c.ProductId == productId);
        }
        public void ClearCart()
        {
            // Get all cart items
            var cartItems = _context.CartItems.ToList();

            // Remove all cart items
            _context.CartItems.RemoveRange(cartItems);
            _context.SaveChanges(); // Save changes to the database
        }
        //public void ClearCart(int cartItemId)
        //{
        //    // Find the cart item bycartItemId 
        //    var cartItem =  _context.CartItems.Find(cartItemId);
        //    if (cartItem != null)
        //    {
        //        _context.CartItems.Remove(cartItem); // Remove the specific cart item
        //        _context.SaveChanges(); // Save the changes to the database
        //    }
        //}


    }

}
