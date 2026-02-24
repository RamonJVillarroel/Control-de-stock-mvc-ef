using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal Precio { get; set; }
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
        public virtual ICollection<TransaccionStock> Transacciones { get; set; } = new List<TransaccionStock>();

        // public DateTime FechaMovimiento { get; set; } = DateTime.Now;
        // public string TipoMovimientoId { get; set; }
    }

    class _ProveedorProductoViewModel
    {
        public List<Producto> Productos { get; set; } = new List<Producto>();
        public List<Proveedor> Proveedores { get; set; } = new List<Proveedor>();
        public List<TransaccionStock> UltimosMovimientos { get; set; }= new List<TransaccionStock>();
    }
}
