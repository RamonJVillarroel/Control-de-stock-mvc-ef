using Control_de_stock_ef.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Control_de_stock_ef.Controllers
{
    [AllowAnonymous]
    public class UsuarioController : Controller
    {
        // Inyección de dependencias para UserManager y SignInManager,
        // que son servicios proporcionados por ASP.NET Core Identity para gestionar usuarios y autenticación.
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
   
        public UsuarioController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
       
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel usuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _signInManager.PasswordSignInAsync(usuario.Email, usuario.Clave, usuario.Recordarme, lockoutOnFailure: false);
                if (resultado.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Inicio de sesión inválido.");
                }
            }
            return View(usuario);
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Protege contra ataques CSRF, ayudando a asegurar que el formulario se envíe desde la misma aplicación.
        public async Task<IActionResult> Registro(RegistroViewModel usuario)
        {
            if (ModelState.IsValid)
            {
                // Lógica para registrar al usuario
                var nuevoUsuario = new Usuario
                {
                    UserName = usuario.Email,
                    Email = usuario.Email,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    ImagenUrlPerfil = "/images/default-avatar.png"
                };
                // registrar al usuario utilizando UserManager
                var resultado = await _userManager.CreateAsync(nuevoUsuario, usuario.Clave);
                if (resultado.Succeeded)
                {
                    //si el resultado es correcto , se inicia sesión automáticamente al nuevo usuario utilizando SignInManager y se redirige a la página de inicio.
                    await _signInManager.SignInAsync(nuevoUsuario, isPersistent: false);
                    //implementacion del servicio de correo electrónico
                    //para enviar un mensaje de bienvenida al nuevo usuario después de registrarse exitosamente.
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in resultado.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }


            }
            return View(usuario);
        }

        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> MiPerfil()
        {
            //esto es para obtener el usuario actual que ha iniciado sesión, utilizando el UserManager de ASP.NET Core Identity.
            //entonces se crea un objeto MiPerfilViewModel con la información del usuario actual (nombre, apellido, email e imagen de perfil) y
            //se pasa a la vista para mostrar el perfil del usuario.
            //no viene porque identity no tiene un método directo para obtener el usuario actual, sino que se obtiene a través del contexto de usuario (User) y luego se consulta la base de datos para obtener los detalles completos del usuario.
            var usuarioActual = await _userManager.GetUserAsync(User);
            var usuarioVM = new MiPerfilViewModel
            {
                Nombre = usuarioActual.Nombre,
                Apellido = usuarioActual.Apellido,
                Email = usuarioActual.Email,
                ImagenUrlPerfil = usuarioActual.ImagenUrlPerfil
            };

            return View(usuarioVM);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MiPerfil(MiPerfilViewModel usuarioVM)
        {
            if (ModelState.IsValid)
            {
                var usuarioActual = await _userManager.GetUserAsync(User);

                try
                {
                   /* if (usuarioVM.ImagenPerfil is not null && usuarioVM.ImagenPerfil.Length > 0)
                    {
                        // opcional: borrar la anterior (si no es placeholder)
                        if (!string.IsNullOrWhiteSpace(usuarioActual.ImagenUrlPerfil))
                            //await _imagenStorage.DeleteAsync(usuarioActual.ImagenUrlPerfil);

                        //var nuevaRuta = await _imagenStorage.SaveAsync(usuarioActual.Id, usuarioVM.ImagenPerfil);
                        usuarioActual.ImagenUrlPerfil = nuevaRuta;
                        usuarioVM.ImagenUrlPerfil = nuevaRuta;
                    }*/
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(usuarioVM);
                }
                // actualizar el nombre y apellido del usuario
                usuarioActual.Nombre = usuarioVM.Nombre;
                usuarioActual.Apellido = usuarioVM.Apellido;
                //usuarioActual.Email = usuarioVM.Email;
                //agregado para updatesolo de usuario nombre y apellido
                usuarioVM.ImagenUrlPerfil = usuarioActual.ImagenUrlPerfil;
                // actualizar el usuario utilizando UserManager y verificar si la actualización fue exitosa. Si es así,
                // se muestra un mensaje de éxito en la vista; de lo contrario, se agregan los errores al ModelState para mostrarlos en la vista.
                var resultado = await _userManager.UpdateAsync(usuarioActual);

                if (resultado.Succeeded)
                {
                    ViewBag.Mensaje = "Perfil actualizado con éxito.";
                    return View(usuarioVM);
                }
                else
                {
                    foreach (var error in resultado.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(usuarioVM);
        }

    }
}
