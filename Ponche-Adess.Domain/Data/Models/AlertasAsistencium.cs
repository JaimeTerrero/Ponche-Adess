using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ponche_Adess.Domain.Data.Models;

public partial class AlertasAsistencium
{
    [Key]
    public int AlertaId { get; set; }

    public int EmpleadoId { get; set; }

    public DateOnly FechaAlerta { get; set; }

    public string TipoAlerta { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual Empleado Empleado { get; set; } = null!;
}
