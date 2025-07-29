using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ponche_Adess.Domain.Data.Models;

public partial class RegistrosAsistencium
{
    [Key]
    public int RegistroId { get; set; }

    public int EmpleadoId { get; set; }

    public DateOnly FechaRegistro { get; set; }

    public TimeOnly HoraEntrada { get; set; }

    public TimeOnly? HoraSalida { get; set; }

    public string? Observacion { get; set; }

    public virtual Empleado Empleado { get; set; } = null!;
}
