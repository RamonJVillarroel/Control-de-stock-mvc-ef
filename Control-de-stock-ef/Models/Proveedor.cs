using System.ComponentModel.DataAnnotations;

namespace Control_de_stock_ef.Models
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public List<Producto>? Productos { get; set; }
    }
}
