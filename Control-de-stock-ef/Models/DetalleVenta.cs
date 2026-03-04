using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Control_de_stock_ef.Models
{
    public class DetalleVenta
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public Venta? Venta { get; set; }
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }
        public int Cantidad { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }
        [Required]
        public string UsuarioId { get; set; } // FK (Identity usa string para el ID)

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; } // Propiedad de navegación
    }
}
