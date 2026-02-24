using Control_de_stock_ef.Data;
using Control_de_stock_ef.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace Control_de_stock_ef.Controllers
{
    public class TransaccionController : Controller
    {
        private readonly ControlDeStockDbContext _context;

        public TransaccionController(ControlDeStockDbContext context)
        {
            _context = context;
        }

        // GET: Transaccion/Create
        // Recibe productoId por si venís desde un botón "Cargar Stock" en la lista de productos
        public IActionResult Create(int? productoId)
        {
            ViewBag.ProductoId = new SelectList(_context.Productos, "Id", "Nombre", productoId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransaccionStock transaccion)
        {
            if (ModelState.IsValid)
            {
                // Usamos una transacción de DB para que no haya fallos parciales
                using var dbTransaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var producto = await _context.Productos.FindAsync(transaccion.ProductoId);
                    if (producto != null)
                    {
                        if (transaccion.Tipo == TipoMovimiento.Entrada)
                            producto.StockActual += transaccion.Cantidad;
                        else
                            producto.StockActual -= transaccion.Cantidad;

                        _context.Add(transaccion);
                        await _context.SaveChangesAsync();
                        await dbTransaction.CommitAsync();

                        return RedirectToAction("Index", "Home"); // Volver al Dashboard
                    }
                }
                catch (Exception)
                {
                    await dbTransaction.RollbackAsync();
                    ModelState.AddModelError("", "Error al procesar el movimiento de stock.");
                }
            }

            ViewBag.ProductoId = new SelectList(_context.Productos, "Id", "Nombre", transaccion.ProductoId);
            return View(transaccion);
        }
    }
}
