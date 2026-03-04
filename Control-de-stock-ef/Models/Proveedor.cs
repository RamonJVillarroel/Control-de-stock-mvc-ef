using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Control_de_stock_ef.Models
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public List<Producto>? Productos { get; set; }
        [Required]
        public string UsuarioId { get; set; } // FK (Identity usa string para el ID)

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; } // Propiedad de navegación
    }
}
