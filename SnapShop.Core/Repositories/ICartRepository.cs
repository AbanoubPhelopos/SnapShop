namespace SnapShop.Core.Repositories
{
    public interface ICartRepository
    {
        // Add a cart item to the user's cart
        void AddToCart( int productId, int quantity);

        // Get all items in a user's cart
        List<CartItem> GetCartItems();
        CartItem GetCartItem(int productId);
        CartItem GetCartItemById(int cartItemId);
        // Update quantity for a specific item in the cart
        void UpdateCartItem(int cartItemId, int quantity);

        // Remove a cart item by its ID
        void RemoveFromCart(int cartItemId);

        // Clear the user's cart after checkout
        void ClearCart();
        //void ClearCart();
        

    }

}
