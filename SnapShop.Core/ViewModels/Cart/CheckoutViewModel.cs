namespace SnapShop.Core.ViewModels.Cart
{
    public class CheckoutViewModel
    {
        public int OrderId { get; set; }
        public List<CartItem>? CartItems { get; set; }
        public decimal TotalAmount { get; set; }
    }
    
}
