using Microsoft.AspNetCore.Mvc;
using Ponche_Adess.Application.Auth.DTOs;
using Ponche_Adess.Application.Auth.Services;

namespace Ponche_Adess.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public AuthController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            var result = await _usuarioService.LoginAsync(model);
            if (result == null)
            {
                ViewBag.Error = "Credenciales inválidas.";
                return View();
            }

            // Guardar el token en sesión o cookie
            HttpContext.Session.SetString("jwt", result.Token);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("jwt");
            return RedirectToAction("Login");
        }
    }
}
