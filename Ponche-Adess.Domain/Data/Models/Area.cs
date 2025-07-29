using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ponche_Adess.Domain.Data.Models;

public partial class Area
{
    [Key]
    public int AreaId { get; set; }

    public string NombreArea { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
