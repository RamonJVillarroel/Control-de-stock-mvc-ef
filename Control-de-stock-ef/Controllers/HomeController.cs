using System.Diagnostics;
using Control_de_stock_ef.Data;
using Control_de_stock_ef.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Control_de_stock_ef.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ControlDeStockDbContext _context;

        public HomeController(ILogger<HomeController> logger,ControlDeStockDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Traemos los productos (podés limitarlos si el dashboard es solo un resumen)
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .ToListAsync();

            var proveedores = await _context.Proveedores.ToListAsync();

            // 2. Traemos los 5 movimientos más nuevos de TODA la app
            var ultimosMovimientos = await _context.TransaccionesStock
                .Include(t => t.Producto) // Importante para mostrar el nombre del producto
                .OrderByDescending(t => t.Fecha)
                .Take(5)
                .ToListAsync();

            var viewModel = new _ProveedorProductoViewModel
            {
                Productos = productos,
                Proveedores = proveedores,
                UltimosMovimientos = ultimosMovimientos // Los pasamos a la vista
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
