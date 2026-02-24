using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Control_de_stock_ef.Data;
using Control_de_stock_ef.Models;

namespace Control_de_stock_ef.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ControlDeStockDbContext _context;

        public ProductosController(ControlDeStockDbContext context)
        {
            _context = context;
        }
       
        // GET: Productos
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["SkuSortParm"] = sortOrder == "sku" ? "sku_desc" : "sku";
            ViewData["PrecioSortParm"] = sortOrder == "precio" ? "precio_desc" : "precio";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var productos = _context.Productos.Include(p => p.Categoria).Include(p => p.Proveedor).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(p => p.Nombre.Contains(searchString) || p.Sku.Contains(searchString) || p.Categoria.Nombre.Contains(searchString) || p.Proveedor.Nombre.Contains(searchString));
            }

            productos = sortOrder switch
            {
                "name_desc" => productos.OrderByDescending(p => p.Nombre),
                "sku" => productos.OrderBy(p => p.Sku),
                "sku_desc" => productos.OrderByDescending(p => p.Sku),
                "precio" => productos.OrderBy(p => p.Precio),
                "precio_desc" => productos.OrderByDescending(p => p.Precio),
                _ => productos.OrderBy(p => p.Nombre),
            };

            int pageSize = 5;
            return View(await PaginatedList<Producto>.CreateAsync(productos.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre");
            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Nombre");
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,Sku,Precio,StockActual,StockMinimo,CategoriaId,ProveedorId")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Nombre", producto.ProveedorId);
            return View(producto);
        }*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,Sku,Precio,StockActual,StockMinimo,CategoriaId,ProveedorId")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                // Iniciamos una transacción para que ambas inserciones sean atómicas
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Guardamos el producto primero
                    _context.Add(producto);
                    await _context.SaveChangesAsync();

                    // 2. Creamos la transacción de stock inicial
                    var stockInicial = new TransaccionStock
                    {
                        ProductoId = producto.Id, // EF ya llenó el Id después del SaveChanges anterior
                        Cantidad = producto.StockActual,
                        Tipo = TipoMovimiento.Entrada,
                        Fecha = DateTime.Now,
                        Motivo = "Carga inicial de producto"
                    };

                    _context.TransaccionesStock.Add(stockInicial);
                    await _context.SaveChangesAsync();

                    // 3. Confirmamos los cambios en la DB
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    // Si algo falla (ej. error de base de datos), deshacemos todo
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "No se pudo crear el producto. Intente nuevamente.");
                }
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Nombre", producto.ProveedorId);
            return View(producto);
        }
        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Nombre", producto.ProveedorId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Sku,Precio,StockActual,StockMinimo,CategoriaId,ProveedorId")] Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Nombre", producto.ProveedorId);
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}
