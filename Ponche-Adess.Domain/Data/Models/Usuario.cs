using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ponche_Adess.Domain.Data.Models;

public partial class Usuario
{
    [Key]
    public int UsuarioId { get; set; }

    public int EmpleadoId { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string ContrasenaHash { get; set; } = null!;

    public bool Estado { get; set; }

    public virtual Empleado Empleado { get; set; } = null!;
}
