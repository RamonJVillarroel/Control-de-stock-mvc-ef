using Control_de_stock_ef.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Control_de_stock_ef.Data
{
    public class ControlDeStockDbContext : IdentityDbContext 
    {
        public ControlDeStockDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<TransaccionStock> TransaccionesStock { get; set; }
    }
}
