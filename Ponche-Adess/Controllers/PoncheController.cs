using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Ponche_Adess.Application.Interfaces;
using Ponche_Adess.Domain.Models;

namespace Ponche_Adess.Controllers
{
    public class PoncheController : Controller
    {
        private readonly IPoncheService _poncheService;

        public PoncheController(IPoncheService poncheService)
        {
            _poncheService = poncheService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarArchivo(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                ViewBag.Error = "Por favor seleccione un archivo .xlsb válido.";
                return View("Index");
            }

            List<RegistroPonche> registros;

            using (var stream = archivo.OpenReadStream())
            {
                registros = LeerExcel(stream);
            }

            var resumen = _poncheService.CalcularResumen(registros);

            return View("Resumen", resumen);
        }

        private List<RegistroPonche> LeerExcel(Stream stream)
        {
            var registros = new List<RegistroPonche>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            // Saltar encabezado
            reader.Read();

            while (reader.Read())
            {
                try
                {
                    var fechaStr = reader.GetValue(2)?.ToString();

                    // Validar si la fila tiene una fecha real
                    if (!DateTime.TryParse(fechaStr, out DateTime fecha))
                        continue;

                    // Manejo seguro de tiempo
                    TimeSpan ingreso = TryParseTime(reader.GetValue(4));
                    TimeSpan salida = TryParseTime(reader.GetValue(5));
                    TimeSpan trabajoReal = TryParseTime(reader.GetValue(6));
                    TimeSpan ingresoTarde = TryParseTime(reader.GetValue(7));
                    TimeSpan salidaAnticipada = TryParseTime(reader.GetValue(8));

                    registros.Add(new RegistroPonche
                    {
                        NumeroEmpleado = reader.GetValue(0)?.ToString(),
                        Nombre = reader.GetValue(1)?.ToString(),
                        Fecha = fecha,
                        Dia = reader.GetValue(3)?.ToString(),
                        Ingreso = TryParseTime(reader.GetValue(4)),
                        Salida = TryParseTime(reader.GetValue(5)),
                        TrabajoReal = TryParseTime(reader.GetValue(6)),
                        IngresoTarde = TryParseTime(reader.GetValue(7)),
                        SalidaAnticipada = TryParseTime(reader.GetValue(8)),
                        Seccion = reader.GetValue(9)?.ToString()
                    });
                }
                catch
                {
                    continue;
                }
            }

            return registros;
        }

        // NUEVO MÉTODO UTILITARIO
        private TimeSpan TryParseTime(object input)
        {
            if (input == null || input.ToString().Trim() == "0")
                return TimeSpan.Zero;

            // Si es TimeSpan directamente
            if (input is TimeSpan ts)
                return ts;

            // Si viene como DateTime (pasa cuando el Excel guarda horas como fecha)
            if (input is DateTime dt)
                return dt.TimeOfDay;

            // Si viene como double (excel guarda hora como decimal 0.5 = 12:00pm)
            if (input is double d)
                return TimeSpan.FromDays(d);

            // Si es string
            var str = input.ToString();
            if (TimeSpan.TryParse(str, out var parsed))
                return parsed;

            return TimeSpan.Zero;
        }

    }
}
