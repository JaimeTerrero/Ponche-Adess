using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ponche_Adess.Domain.Models
{
    public class RegistroPonche
    {
        public string NumeroEmpleado { get; set; }
        public string Nombre { get; set; }
        public DateTime Fecha { get; set; }
        public string Dia { get; set; }
        public TimeSpan Ingreso { get; set; }
        public TimeSpan Salida { get; set; }
        public TimeSpan TrabajoReal { get; set; }
        public TimeSpan IngresoTarde { get; set; }
        public TimeSpan SalidaAnticipada { get; set; }
        public string Seccion { get; set; }
    }
}
