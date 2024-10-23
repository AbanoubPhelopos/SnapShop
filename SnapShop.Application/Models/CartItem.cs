using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapShop.Application.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price => Product.Price;  // Derived from the Product
        public decimal TotalPrice => Quantity * Price;
        public Product Product { get; set; }  // Reference to the Product object
        [ForeignKey("Order")]
        public int? OrderId { get; set; }  // This represents the foreign key
        public Order ?Order { get; set; }  // Navigation property to the Order
    }

}
