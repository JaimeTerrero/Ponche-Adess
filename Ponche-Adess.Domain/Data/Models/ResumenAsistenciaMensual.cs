using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ponche_Adess.Domain.Data.Models;

public partial class ResumenAsistenciaMensual
{
    [Key]
    public int ResumenId { get; set; }

    public int EmpleadoId { get; set; }

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
}
