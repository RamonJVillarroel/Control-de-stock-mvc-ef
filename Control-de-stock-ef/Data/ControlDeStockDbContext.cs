using Control_de_stock_ef.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Control_de_stock_ef.Data
{
    public class ControlDeStockDbContext : IdentityDbContext<Usuario>
    {
        public ControlDeStockDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<TransaccionStock> TransaccionesStock { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }
        //public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Obtener todas las relaciones (Foreign Keys) de todos los modelos
            var foreignKeys = builder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys());

            foreach (var relationship in foreignKeys)
            {
                // Cambiar el comportamiento de borrado a 'Restrict' por defecto
                // Esto evita los "Multiple Cascade Paths" en SQL Server
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
