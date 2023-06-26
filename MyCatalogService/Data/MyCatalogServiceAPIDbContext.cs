using Microsoft.EntityFrameworkCore;
using MyCatalogService.Model;
using System.Collections.Generic;

namespace MyCatalogService.Data
{
    public class MyCatalogServiceAPIDbContext : DbContext
    {
        public MyCatalogServiceAPIDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
    }
}
