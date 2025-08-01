using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ponche_Adess.Application.Auth.DTOs
{
    public class LoginRequestDto
    {
        [DisplayName("Usuario")]
        public string NombreUsuario { get; set; } = null!;
        [DisplayName("Contraseña")]
        public string Contrasena { get; set; } = null!;
    }
}
