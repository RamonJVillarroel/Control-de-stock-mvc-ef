using Control_de_stock_ef.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Control_de_stock_ef.Models;
namespace Control_de_stock_ef.Controllers
{
    public class TransaccionController : Controller
    {
        private readonly ControlDeStockDbContext _context;

        public TransaccionController(ControlDeStockDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(TransaccionStock transaccion)
        {
            if (ModelState.IsValid)
            {
                var producto = await _context.Productos.FindAsync(transaccion.ProductoId);
                if (producto != null)
                {
                    // Actualizar el stock físico en la tabla Producto
                    if (transaccion.Tipo == TipoMovimiento.Entrada)
                        producto.StockActual += transaccion.Cantidad;
                    else
                        producto.StockActual -= transaccion.Cantidad;

                    _context.Add(transaccion);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(transaccion);
        }
    }
}
