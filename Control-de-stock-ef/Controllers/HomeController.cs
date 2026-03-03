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
            
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .ToListAsync();

            var proveedores = await _context.Proveedores.ToListAsync();

            var ultimosMovimientos = await _context.TransaccionesStock
                .Include(t => t.Producto)
                .OrderByDescending(t => t.Fecha)
                .Take(5)
                .ToListAsync();

            var datosInventario = new _ProveedorProductoViewModel
            {
                Productos = productos,
                Proveedores = proveedores,
                UltimosMovimientos = ultimosMovimientos
            };

            
            var ventas = await _context.Ventas
                .Include(v => v.Cliente)
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();

            var datosVentas = new DashboardVentasVM
            {
                TotalVentasCount = ventas.Count,
                IngresosTotales = ventas.Sum(v => v.PrecioTotal),
                TicketPromedio = ventas.Count > 0 ? ventas.Average(v => v.PrecioTotal) : 0,
                VentasRecientes = ventas.Take(5).ToList()
            };

            
            var viewModelFinal = new HomeDashboardVM
            {
                Inventario = datosInventario,
                Ventas = datosVentas
            };

            return View(viewModelFinal);
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
