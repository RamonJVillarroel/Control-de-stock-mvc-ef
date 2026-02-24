using Control_de_stock_ef.Data;
using Control_de_stock_ef.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Control_de_stock_ef.Controllers
{
    public class VentasController : Controller
    {
        private readonly ControlDeStockDbContext _context;

        public VentasController(ControlDeStockDbContext context)
        {
            _context = context;
        }

        // GET: Ventas/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre");
            
            var productos = _context.Productos
                .Select(p => new {
                    p.Id,
                    p.Nombre,
                    p.Precio,
                    p.StockActual
                }).ToList();

            ViewBag.Productos = productos;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venta venta, int[] productoId, int[] cantidad)
        {
            if (productoId == null || productoId.Length == 0)
            {
                ModelState.AddModelError("", "Debes agregar al menos un producto a la venta.");
            }

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    decimal totalVenta = 0;

                    // 1. Procesar cada producto del "carrito"
                    for (int i = 0; i < productoId.Length; i++)
                    {
                        var pId = productoId[i];
                        var cant = cantidad[i];
                        var producto = await _context.Productos.FindAsync(pId);

                        if (producto == null || producto.StockActual < cant)
                        {
                            throw new Exception($"Stock insuficiente para {producto?.Nombre}");
                        }

                        // Calcular subtotal
                        var subtotal = producto.Precio * cant;
                        totalVenta += subtotal;

                        // 2. Crear el Detalle de Venta
                        var detalle = new DetalleVenta
                        {
                            ProductoId = pId,
                            Cantidad = cant,
                            PrecioUnitario = producto.Precio,
                            Venta = venta
                        };
                        _context.DetallesVenta.Add(detalle);

                        // 3. Generar la Transacción de Stock (Salida)
                        var movimiento = new TransaccionStock
                        {
                            ProductoId = pId,
                            Cantidad = cant,
                            Tipo = TipoMovimiento.Salida,
                            Fecha = DateTime.Now,
                            Motivo = $"Venta Factura #{venta.Id}"
                        };
                        _context.TransaccionesStock.Add(movimiento);

                        // 4. Actualizar Stock físico del producto
                        producto.StockActual -= cant;
                    }

                    // 5. Completar datos de la Venta y actualizar Cliente
                    venta.PrecioTotal = totalVenta;
                    venta.FechaVenta = DateTime.Now;
                    _context.Add(venta);

                    var cliente = await _context.Clientes.FindAsync(venta.ClienteId);
                    if (cliente != null)
                    {
                        cliente.Monto += totalVenta;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Error: " + ex.Message);
                }
            }

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", venta.ClienteId);
            return View(venta);
        }
    }
}
