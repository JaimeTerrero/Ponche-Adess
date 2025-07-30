using System;
using System.Collections.Generic;

namespace Ponche_Adess.Domain.Data.Models;

public partial class ResumenAsistenciaMensual
{
    public int ResumenId { get; set; }

    public string Seccion { get; set; } = null!;

    public int DiasTotales { get; set; }

    public int DiasTarde { get; set; }

    public int MinTarde { get; set; }

    public int DiasAnticipados { get; set; }

    public int MinAnticipados { get; set; }

    public decimal HorasTrabajadas { get; set; }

    public decimal PorcCumplimiento { get; set; }

    public int Mes { get; set; }

    public int Anio { get; set; }

    public string Empleado { get; set; } = null!;
}
