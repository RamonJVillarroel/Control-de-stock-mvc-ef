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
            var viewModelFinal = await ObtenerDatosDashboardAsync();
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
        public async Task<IActionResult> DescargarPdf()
        {
            var model = await ObtenerDatosDashboardAsync();

            return new ViewAsPdf("ReporteVentasInventarioPdf", model) // Cambiamos "Index" por "ReportePdf"
            {
                FileName = $"Reporte_Stock_ventas_{DateTime.Now:yyyyMMdd}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                CustomSwitches = "--print-media-type" // Para que intente cargar estilos de impresión
            };
        }


        private async Task<HomeDashboardVM> ObtenerDatosDashboardAsync()
        {
            var userId = _userManager.GetUserId(User);

            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Where(p => p.UsuarioId == userId)
                .ToListAsync();

            var proveedores = await _context.Proveedores
                .Where(p => p.UsuarioId == userId)
                .ToListAsync();

            var ultimosMovimientos = await _context.TransaccionesStock
                .Include(t => t.Producto)
                .Where(t => t.UsuarioId == userId)
                .OrderByDescending(t => t.Fecha)
                .Take(5)
                .ToListAsync();

            var ventas = await _context.Ventas
                .Include(v => v.Cliente)
                .Where(v => v.UsuarioId == userId)
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();

            return new HomeDashboardVM
            {
                Inventario = new _ProveedorProductoViewModel
                {
                    Productos = productos,
                    Proveedores = proveedores,
                    UltimosMovimientos = ultimosMovimientos
                },
                Ventas = new DashboardVentasVM
                {
                    TotalVentasCount = ventas.Count,
                    IngresosTotales = ventas.Sum(v => v.PrecioTotal),
                    TicketPromedio = ventas.Count > 0 ? ventas.Average(v => v.PrecioTotal) : 0,
                    VentasRecientes = ventas.Take(5).ToList()
                }
            };
        }

    }
}
