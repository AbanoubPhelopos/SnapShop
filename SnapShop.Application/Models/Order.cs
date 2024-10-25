using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapShop.Application.Models
{
    public class Order
    {
        [Key] // Specifies that this is the primary key
        [Column("OrderId")]
        public int OrderId { get; set; } // Auto-incremented unique order number

        [Required(ErrorMessage = "Order date is required.")]
        [DataType(DataType.Date)] // Formats the date
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Total amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than zero.")]
        [DataType(DataType.Currency)] // Formats the decimal as currency
        public decimal TotalAmount { get; set; }

        // Relationship to cart items
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // Initialize the list to avoid null reference issues
    }
}
