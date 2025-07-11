using Ponche_Adess.Application.Interfaces;
using Ponche_Adess.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ponche_Adess.Application.Services
{
    public class PoncheService : IPoncheService
    {
        public List<ResumenEmpleado> CalcularResumen(List<RegistroPonche> registros)
        {
            var horaEntrada = new TimeSpan(8, 0, 0); // 8:00 AM

            var resumen = registros
                .Where(r => !string.IsNullOrWhiteSpace(r.NumeroEmpleado))
                .GroupBy(r => new { r.NumeroEmpleado, r.Nombre, r.Seccion })
                .Select(g =>
                {
                    int totalDias = g.Count();

                    // ✅ LÓGICA CLARA: entrada válida y mayor que 08:00
                    var diasTarde = g.Count(r =>
                        r.Ingreso != TimeSpan.Zero &&
                        r.Ingreso > horaEntrada
                    );

                    var minutosTarde = g
                        .Where(r => r.Ingreso != TimeSpan.Zero && r.Ingreso > horaEntrada)
                        .Sum(r => (r.Ingreso - horaEntrada).TotalMinutes);

                    var diasAnticipados = g.Count(r => r.SalidaAnticipada > TimeSpan.Zero);
                    var minutosAnticipados = g.Sum(r => r.SalidaAnticipada.TotalMinutes);
                    var horasTrabajadas = g.Sum(r => r.TrabajoReal.TotalHours);

                    return new ResumenEmpleado
                    {
                        NumeroEmpleado = g.Key.NumeroEmpleado,
                        Nombre = g.Key.Nombre,
                        Seccion = g.Key.Seccion,
                        TotalDias = totalDias,
                        DiasTarde = diasTarde,
                        MinutosTotalesTarde = minutosTarde,
                        DiasSalidaAnticipada = diasAnticipados,
                        MinutosTotalesAnticipados = minutosAnticipados,
                        HorasTrabajadasTotales = horasTrabajadas
                    };
                })
                .OrderBy(r => r.Nombre)
                .ToList();

            return resumen;
        }




    }
}
