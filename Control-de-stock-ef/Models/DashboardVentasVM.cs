namespace Control_de_stock_ef.Models
{
    public class DashboardVentasVM
    {
        public int TotalVentasCount { get; set; }
        public decimal IngresosTotales { get; set; }
        public decimal TicketPromedio { get; set; }
        public List<Venta> VentasRecientes { get; set; }

    }
    
}
