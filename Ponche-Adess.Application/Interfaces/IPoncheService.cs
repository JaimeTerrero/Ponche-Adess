using Ponche_Adess.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ponche_Adess.Application.Interfaces
{
    public interface IPoncheService
    {
        List<ResumenEmpleado> CalcularResumen(List<RegistroPonche> registros);
    }
}
