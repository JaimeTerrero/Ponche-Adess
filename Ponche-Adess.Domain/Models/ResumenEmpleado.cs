using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ponche_Adess.Domain.Models
{
    public class ResumenEmpleado
    {
        public string NumeroEmpleado { get; set; }
        public string Nombre { get; set; }
        public string Seccion { get; set; }

        public int TotalDias { get; set; }
        public int DiasTarde { get; set; }
        public double MinutosTotalesTarde { get; set; }
        public int DiasSalidaAnticipada { get; set; }
        public double MinutosTotalesAnticipados { get; set; }
        public double HorasTrabajadasTotales { get; set; }

        public string Cumplimiento => TotalDias > 0
            ? $"{(HorasTrabajadasTotales / (TotalDias * 8.0) * 100):F2}%"
            : "N/A";
    }
}
