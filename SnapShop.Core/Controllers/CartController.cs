using Microsoft.AspNetCore.Authorization;
using SnapShop.Core.ViewModels.Cart;
using SnapShop.Utility;
using Stripe.Checkout;

namespace SnapShop.Core.Controllers
{
	[Authorize(Roles = StaticDetails.RoleUserCashier + "," + StaticDetails.RoleUserManager)]
    public class CartController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        public CartController(ICartRepository cartRepository, IProductRepository productRepository, ICategoryRepository categoryRepository, IOrderRepository orderRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _orderRepository = orderRepository;
        }
        public IActionResult OrderDetails(int id)
        {
            var order = _orderRepository.GetOrderWithItems(id);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("GetAllOrders");
            }

            return View(order); // This will return the order details view
        }

        public IActionResult GetAllOrders()
        {
            var orderItems = _orderRepository.GetAllOrders();
            return View(orderItems);
        }
        public IActionResult GetAll()
        {
            var cartItems = _cartRepository.GetCartItems();
            return View(cartItems);
        }
        // Display cart items
        public IActionResult ViewCart()
        {
            ViewData["ProductList"] = _productRepository.GetProducts();
            ViewData["Categories"] = _categoryRepository.GetCategories();

            var cartItems = _cartRepository.GetCartItems();

            // Calculate total price
            var total = cartItems.Sum(item => item.Product.Price * item.Quantity);

            // Pass total as ViewBag or include it in a ViewModel
            ViewBag.Total = total;

            return View(cartItems);
        }
        [HttpGet]
        public IActionResult GetProductsByCategory(int categoryId)
        {
            var products = _productRepository.GetProductsByCategory(categoryId);  // Fetch products by category
            var productList = products.Select(p => new
            {
                id = p.Id,
                name = p.Name
            });

            return Json(productList);  // Return products as JSON for the dropdown
        }
        // Add item to cart
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                // Quantity is invalid
                TempData["ErrorMessage"] = "Quantity must be a positive number.";
                return RedirectToAction("GetProduct", "Cashier", new { id = productId });
            }

            var product = _productRepository.GetById(productId);

            if (product == null)
            {
                // Product does not exist
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("GetProduct", "Cashier", new { id = productId });
            }

            if (product.Quantity < quantity)
            {
                // Not enough stock
                TempData["ErrorMessage"] = "Insufficient stock available.";
                return RedirectToAction("GetProduct", "Cashier", new { id = productId });
            }

            // Check if the product is already in the cart
            var existingCartItem = _cartRepository.GetCartItem(productId); // Assume this method exists in your repository

            if (existingCartItem != null)
            {
                // Product is already in the cart
                TempData["ErrorMessage"] = "Product is already in the cart.";
                return RedirectToAction("GetProduct", "Cashier", new { id = productId });
            }

            // Add the product to the cart if it is not already there
            _cartRepository.AddToCart(productId, quantity);
            TempData["SuccessMessage"] = "Product added to cart successfully.";
            return RedirectToAction("GetProduct", "Cashier", new { id = productId });
        }

        // Update item quantity in cart
        [HttpPost]
        public IActionResult UpdateCartItem(int cartItemId, int quantity)
        {
            if (quantity <= 0)
            {
                // Invalid quantity
                TempData["ErrorMessage"] = "Quantity must be a positive number.";

                // Load cart items and category options to pass back to the view
                var cartItems = _cartRepository.GetCartItems();

                return RedirectToAction("ViewCart");  // Return the view with cart data
            }
            var cartItem = _cartRepository.GetCartItemById(cartItemId);
            if (cartItem == null)
            {
                TempData["ErrorMessage"] = "Cart item not found.";
                return RedirectToAction("ViewCart");
            }
            var product = _productRepository.GetById(cartItem.ProductId);
            if (product == null || product.Quantity < quantity)
            {
                // Product not found or insufficient stock
                TempData["ErrorMessage"] = "Insufficient stock available or product no longer exists.";

                var cartItems = _cartRepository.GetCartItems();

                return RedirectToAction("ViewCart");  // Return the view with cart data
            }
            try
            {
                // Update the cart item with the valid quantity
                _cartRepository.UpdateCartItem(cartItemId, quantity);
                TempData["SuccessMessage"] = "Cart item updated successfully!";
            }
            catch (Exception)
            {
                // Log the error if necessary
                TempData["ErrorMessage"] = "An error occurred while updating the cart item.";
            }
            return RedirectToAction("ViewCart");
        }

        // Remove item from cart
        [HttpPost]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            _cartRepository.RemoveFromCart(cartItemId);
            return RedirectToAction("ViewCart");
        }
        [HttpPost]
        public IActionResult DeleteOrder(int orderId)
        {
	        Console.WriteLine("DeleteOrder called with Order ID: " + orderId); // Debug line

	        var order = _orderRepository.GetOrderById(orderId);

	        if (order == null)
	        {
		        return Json(new { success = false, message = "Order not found." });
	        }

	        try
	        {
		        _orderRepository.DeleteOrder(orderId);
		        return Json(new { success = true, message = "Order deleted successfully!" });
	        }
	        catch (Exception ex)
	        {
		        Console.WriteLine("Error while deleting order: " + ex.Message); // Debug line
		        return Json(new { success = false, message = "An error occurred while deleting the order." });
	        }
        }

		public IActionResult Confirmation()
		{
			var cartItems = _cartRepository.GetCartItems(); // Fetch cart items for confirmation
			var total = cartItems.Sum(item => item.TotalPrice);
            var orderItems = cartItems.Select(cartItem => new OrderItem
            {
                ProductId = cartItem.Product.Id,
                Product = cartItem.Product,
                ProductName = cartItem.Product.Name,
                Quantity = cartItem.Quantity,
                Price = cartItem.Price,
                TotalPrice = cartItem.TotalPrice,
                Image = cartItem.Product.Image
            }).ToList();

            // Create a new order
            var newOrder = new Order
            {
                OrderDate = DateTime.UtcNow,
                TotalAmount = total,
                OrderItems = orderItems
            };
            _orderRepository.AddOrder(newOrder); // Persist the new order
         
            // Create CheckoutViewModel for confirmation view
            var confirmationModel = new CheckoutViewModel
			{
				CartItems = cartItems,
				TotalAmount = total,
				OrderId = newOrder.OrderId // Assuming OrderId is part of CheckoutViewModel
			};
            foreach(var item in cartItems)
            {
                item.Product.Quantity -= item.Quantity;
            }
            
			// Clear the cart
			_cartRepository.ClearCart();

			return View(confirmationModel);
		}

		[HttpGet]
		public IActionResult Checkout()
		{
			var cartItems = _cartRepository.GetCartItems(); // Fetch cart items
			if (cartItems == null || !cartItems.Any())
			{
				TempData["ErrorMessage"] = "No items found in the cart.";
				return RedirectToAction("ViewCart"); // Redirect to cart if no items are present
			}

			var total = cartItems.Sum(item => item.TotalPrice);

			var checkoutModel = new CheckoutViewModel
			{
				CartItems = cartItems,
				TotalAmount = total
			};

			return View(checkoutModel); // Return the checkout view with cart items and total
		}


		[HttpPost]
		public IActionResult Checkout(CheckoutViewModel model)
		{
			var cartItems = _cartRepository.GetCartItems(); // Fetch cart items
			if (cartItems == null || !cartItems.Any())
			{
				ModelState.AddModelError("", "No items found in the cart.");
				return RedirectToAction("ViewCart"); // Return the view with an error message
			}

			// Calculate total amount in USD
			var total = cartItems.Sum(item => item.TotalPrice);

			if (total < 0.50m) // Assuming total is in USD
			{
				TempData["ErrorMessage"] = "The total amount must be at least $0.50.";
				return RedirectToAction("ViewCart"); // Redirect to the cart view with an error message
			}

			// Create a new Stripe Checkout session
			var domain = "http://localhost:5081/"; // Update with your domain
			var options = new SessionCreateOptions
			{
				SuccessUrl = $"{domain}Cart/Confirmation",
				CancelUrl = $"{domain}Cart/ViewCart",
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment"
			};

			foreach (var item in cartItems)
			{
				var sessionListItem = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Product.Price * 100), // Convert to cents
						Currency = "usd", // Use USD directly
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.Name,
						}
					},
					Quantity = item.Quantity,
				};
				options.LineItems.Add(sessionListItem);
			}

			var service = new SessionService();
			Session session = service.Create(options);
			Response.Headers.Add("Location", session.Url);

			return new StatusCodeResult(303); // Redirect to Stripe Checkout
		}



	}

}