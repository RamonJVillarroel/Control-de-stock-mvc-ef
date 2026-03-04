using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Control_de_stock_ef.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        [Required]
        public string UsuarioId { get; set; } // FK (Identity usa string para el ID)

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; } // Propiedad de navegación
        public List<Producto>? Productos { get; set; }

    }
}
