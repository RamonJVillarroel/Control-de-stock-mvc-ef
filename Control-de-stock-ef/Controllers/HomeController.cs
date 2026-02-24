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

            var productos = await _context.Productos.Include(p => p.Proveedor).ToListAsync();
            var proveedores = await _context.Proveedores.ToListAsync();

            // Creamos el ViewModel que la vista está esperando
            var viewModel = new _ProveedorProductoViewModel
            {
                Productos =  productos,
                Proveedores =  proveedores,
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
