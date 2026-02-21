using Control_de_stock_ef.Models;

namespace Control_de_stock_ef.Data
{
    public class DbSeeder
    {
        public static void Seed(ControlDeStockDbContext context)
        {
            // 1. Aseguramos que la base de datos esté creada
            context.Database.EnsureCreated();

            // 2. Cargar Categorías si no existen
            if (!context.Categorias.Any())
            {
                context.Categorias.AddRange(
                    new Categoria { Nombre = "Electrónica" },
                    new Categoria { Nombre = "Alimentos" },
                    new Categoria { Nombre = "Oficina" }
                );
                context.SaveChanges();
            }

            // 3. Cargar Proveedores
            if (!context.Proveedores.Any())
            {
                context.Proveedores.AddRange(
                    new Proveedor { Nombre = "Tech S.A.", Telefono = "11223344", Email = "ventas@tech.com" },
                    new Proveedor { Nombre = "Distribuidora Global", Telefono = "55667788", Email = "info@global.com" }
                );
                context.SaveChanges();
            }

            // 4. Cargar Productos iniciales (Relacionados)
            if (!context.Productos.Any())
            {
                var electronica = context.Categorias.First(c => c.Nombre == "Electrónica");
                var techSa = context.Proveedores.First(p => p.Nombre == "Tech S.A.");

                context.Productos.AddRange(
                    new Producto
                    {
                        Nombre = "Mouse Gamer",
                        Descripcion = "Mouse con alta precisión y luces RGB",
                        Sku = "MOU-001",
                        Precio = 2500,
                        StockActual = 15,
                        CategoriaId = electronica.Id,
                        ProveedorId = techSa.Id
                    },
                    new Producto
                    {
                        Nombre = "Teclado Mecánico",
                        Descripcion = "Teclado con switches mecánicos y retroiluminación",
                        Sku = "TEC-002",
                        Precio = 5800,
                        StockActual = 3, // Esto saldría en rojo según el prompt de Stitch ;)
                        CategoriaId = electronica.Id,
                        ProveedorId = techSa.Id
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
