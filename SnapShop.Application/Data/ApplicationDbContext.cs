using Microsoft.EntityFrameworkCore;
using SnapShop.Application.Models;

namespace SnapShop.Application.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Category?> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}