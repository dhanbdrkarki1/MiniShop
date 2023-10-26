using Microsoft.EntityFrameworkCore;
using MiniShop.Models.Entity;
using System;

namespace MiniShop.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
    }
}
