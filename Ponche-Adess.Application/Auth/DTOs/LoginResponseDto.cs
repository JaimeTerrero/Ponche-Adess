using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ponche_Adess.Application.Auth.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public string NombreUsuario { get; set; } = null!;
        public int UsuarioId { get; set; }
        public string? Rol { get; set; } // Si deseas incluir un campo de rol o privilegios más adelante
    }
}
