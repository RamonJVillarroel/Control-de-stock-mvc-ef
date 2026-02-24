namespace Control_de_stock_ef.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public decimal Monto { get; set; } = 0;
        public string EstadoCliente { get; set; } = "Activo";
    }
}
