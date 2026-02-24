using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Control_de_stock_ef.Models
{
    // 1. El Enum con un nombre distinto para evitar confusión
    public enum TipoMovimiento
    {
        Entrada = 1,
        Salida = 2,
        Ajuste = 3
    }

    // 2. La Clase que representa la tabla en la DB
    public class TransaccionStock
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Producto")]
        public int ProductoId { get; set; }
        public virtual Producto? Producto { get; set; }

        [Required]
        [DisplayName("Tipo de Movimiento")]
        public TipoMovimiento Tipo { get; set; } // Ahora se entiende perfecto

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [StringLength(250)]
        [DisplayName("Motivo u Observación")]
        public string? Motivo { get; set; }
    }
}