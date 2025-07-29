using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ponche_Adess.Domain.Data.Models;

public partial class Empleado
{
    [Key]
    public int EmpleadoId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Cedula { get; set; } = null!;

    public string CorreoElectronico { get; set; } = null!;

    public string? Telefono { get; set; }

    public int AreaId { get; set; }

    public int RolId { get; set; }

    public DateOnly FechaIngreso { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<AlertasAsistencium> AlertasAsistencia { get; set; } = new List<AlertasAsistencium>();

    public virtual Area Area { get; set; } = null!;

    public virtual ICollection<RegistrosAsistencium> RegistrosAsistencia { get; set; } = new List<RegistrosAsistencium>();

    public virtual Role Rol { get; set; } = null!;

    public virtual Usuario? Usuario { get; set; }
}
