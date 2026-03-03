namespace Control_de_stock_ef.Models
{
    public class HomeDashboardVM
    {
        public _ProveedorProductoViewModel Inventario { get; set; }
        public DashboardVentasVM Ventas { get; set; }
    }
}