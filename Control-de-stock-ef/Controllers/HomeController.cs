using Control_de_stock_ef.Data;
using Control_de_stock_ef.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using Rotativa;
using Rotativa.AspNetCore;
using System.Diagnostics;

namespace Control_de_stock_ef.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ControlDeStockDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public HomeController(ILogger<HomeController> logger,ControlDeStockDbContext context, UserManager<Usuario> userManager )
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Where(p=> p.UsuarioId == _userManager.GetUserId(User))
                .ToListAsync();

            var proveedores = await _context.Proveedores
                .Where(p => p.UsuarioId == _userManager.GetUserId(User))
                .ToListAsync();

            var ultimosMovimientos = await _context.TransaccionesStock
                .Include(t => t.Producto)
                .OrderByDescending(t => t.Fecha)
                .Take(5)
                .Where(t => t.UsuarioId == _userManager.GetUserId(User))
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
                .Where(v => v.UsuarioId == _userManager.GetUserId(User))
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
        public ActionResult DescargarPdf()
        {
           
            var model = new HomeDashboardVM
            {
                Inventario = new _ProveedorProductoViewModel
                {
                    Productos = _context.Productos.Include(p => p.Categoria).Include(p => p.Proveedor).Where(p => p.UsuarioId == _userManager.GetUserId(User)).ToList(),
                    Proveedores = _context.Proveedores.Where(p => p.UsuarioId == _userManager.GetUserId(User)).ToList(),
                    UltimosMovimientos = _context.TransaccionesStock.Include(t => t.Producto).OrderByDescending(t => t.Fecha).Take(5).Where(t => t.UsuarioId == _userManager.GetUserId(User)).ToList()
                },
                Ventas = new DashboardVentasVM
                {
                    TotalVentasCount = _context.Ventas.Where(v => v.UsuarioId == _userManager.GetUserId(User)).Count(),
                    IngresosTotales = _context.Ventas.Where(v => v.UsuarioId == _userManager.GetUserId(User)).Sum(v => v.PrecioTotal),
                    TicketPromedio = _context.Ventas.Where(v => v.UsuarioId == _userManager.GetUserId(User)).Count() > 0 ? _context.Ventas.Where(v => v.UsuarioId == _userManager.GetUserId(User)).Average(v => v.PrecioTotal) : 0,
                    VentasRecientes = _context.Ventas.Include(v => v.Cliente).OrderByDescending(v => v.FechaVenta).Take(5).Where(v => v.UsuarioId == _userManager.GetUserId(User)).ToList()
                }
            };

            return new ViewAsPdf("Index", model)
            {
                FileName = "Dashboard.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };

        }


    }
}
