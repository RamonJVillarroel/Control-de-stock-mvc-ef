using System.ComponentModel.DataAnnotations;

namespace Control_de_stock_ef.Models
{
    public class Venta
    {
        public int Id { get; set; }
        [Required]
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }
        public string MetodoPago { get; set; }
        public decimal PrecioTotal { get; set; }
        public DateTime FechaVenta { get; set; }
        public virtual ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
