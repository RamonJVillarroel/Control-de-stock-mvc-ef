using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Control_de_stock_ef.Models
{
    public class Producto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(200)]
        public string Descripcion { get; set; }
        [Required]
        [StringLength(20)]
        public string Sku { get; set; }

        public float Precio { get; set; }
        [Required]
        public int StockActual { get; set; }
        [Required]
        public int StockMinimo { get; set; }
        [DisplayName("Categoría")]
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }
        [DisplayName("Proveedor")]
        public int ProveedorId { get; set; }
        public Proveedor? Proveedor { get; set; }

    }

    class _ProveedorProductoViewModel
    {
        public List<Producto> Productos { get; set; } = new List<Producto>();
        public List<Proveedor> Proveedores { get; set; } = new List<Proveedor>();
    }
}
