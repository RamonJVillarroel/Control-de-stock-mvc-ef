using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Control_de_stock_ef.Models
{
    public class Usuario : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(50)]
        public string Apellido { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
        public string ImagenUrlPerfil { get; set; }
        public List<Categoria>? Categorias { get; set; }
        public List<Proveedor>? Proveedores { get; set; }
        public List<Cliente>? Clientes { get; set; }
        public List<Producto>? Productos { get; set; }
        public List<Venta>? Ventas { get; set; }
        public List<DetalleVenta>? DetalleVentas { get; set; }
        public List<TransaccionStock>? TransaccionesStock { get; set; }

    }

    public class RegistroViewModel
    {
        [Required(ErrorMessage = "Debes ingresar un nombre")]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debes ingresar un apellido")]
        [StringLength(50)]
        public string Apellido { get; set; }

        [EmailAddress(ErrorMessage = "Ingresa un email válido.")]
        [Required(ErrorMessage = "El email es obligatorio.")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "La clave es obligatoria.")]
        public string Clave { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Debes confirmar la clave.")]
        [Compare("Clave", ErrorMessage = "Las claves no coinciden.")]
        public string ConfirmarClave { get; set; }
    }

    public class LoginViewModel
    {
        [EmailAddress(ErrorMessage = "Ingresa un email válido.")]
        [Required(ErrorMessage = "El email es obligatorio.")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "La clave es obligatoria.")]
        public string Clave { get; set; }
        public bool Recordarme { get; set; }
    }

    public class MiPerfilViewModel
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string? Email { get; set; }
        //esta propiedad es para permitir que el usuario suba una nueva imagen de perfil,
        //y la propiedad ImagenUrlPerfil es para mostrar la imagen actual del usuario en su perfil.
        public IFormFile? ImagenPerfil { get; set; }
        public string? ImagenUrlPerfil { get; set; }
    }
}
