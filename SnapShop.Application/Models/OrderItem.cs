using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapShop.Application.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; } // Primary key
        [ForeignKey("Order")]
        public int OrderId { get; set; } // Foreign key linking to the Order
        public Order ?Order { get; set; } // Navigation property
        [ForeignKey("Product")]
        public int ProductId { get; set; } // Foreign key linking to the Product
        public Product Product { get; set; } // Navigation property to the Produc
        public string ProductName { get; set; } // Name of the product
        public string Image { get; set; }
    public decimal Price { get; set; } // Price of the product
        public int Quantity { get; set; } // Quantity of the product in this order
        public decimal TotalPrice { get; set; } // Total price for this item (Price * Quantity)
    }
}
