using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ponche_Adess.Application.Interfaces;
using Ponche_Adess.Domain.Data.Models;
using Ponche_Adess.Domain.Models;
using System.Globalization;

namespace Ponche_Adess.Controllers
{
    public class PoncheController : Controller
    {
        private readonly IPoncheService _poncheService;
        private readonly AppDbContext _context;

        public PoncheController(IPoncheService poncheService, AppDbContext context)
        {
            _poncheService = poncheService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("jwt");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            // Si hay token, renderiza la vista
            return View();
        }

        [HttpGet]
        public IActionResult MesesDisponibles()
        {
            var meses = _context.ResumenAsistenciaMensuals
            .Select(r => new { r.Mes, r.Anio })
            .Distinct()
            .OrderByDescending(x => x.Anio) // año más reciente primero
            .ThenBy(x => x.Mes)             // meses de 1..12 (Enero primero)
            .AsEnumerable()
            .Select(x => (x.Mes, x.Anio))
            .ToList();

            return View(meses);
        }

        // GET: /Ponche/EmpleadoResumen?empleado=Daniel%20Medina
        [HttpGet]
        public IActionResult EmpleadoResumen(string empleado)
        {
            if (string.IsNullOrWhiteSpace(empleado))
                return RedirectToAction(nameof(MesesDisponibles));

            var nombre = empleado.Trim().ToLower();

            var registros = _context.ResumenAsistenciaMensuals
                .Where(x => x.Empleado.ToLower().Trim() == nombre)
                .OrderByDescending(x => x.Anio)
                .ThenBy(x => x.Mes)
                .ToList();

            // Si no hay métricas exactas por coincidencia estricta,
            // intenta búsqueda contains (útil cuando el usuario no teclea el nombre completo)
            if (registros.Count == 0)
            {
                registros = _context.ResumenAsistenciaMensuals
                    .Where(x => x.Empleado.ToLower().Contains(nombre))
                    .OrderByDescending(x => x.Anio)
                    .ThenBy(x => x.Mes)
                    .ToList();
            }

            ViewBag.EmpleadoBuscado = empleado;
            return View("EmpleadoResumen", registros);
        }

        // (Opcional) Sugerencias rápidas de empleados: /Ponche/BuscarEmpleados?q=dan
        [HttpGet]
        public IActionResult BuscarEmpleados(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return Json(Array.Empty<string>());

            var query = q.Trim().ToLower();

            var empleados = _context.ResumenAsistenciaMensuals
                .Where(x => x.Empleado.ToLower().Contains(query))
                .Select(x => x.Empleado)
                .Distinct()
                .OrderBy(x => x)
                .Take(10)
                .ToList();

            return Json(empleados);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarMes(int mes, int anio)
        {
            // (Opcional) seguridad básica: solo años razonables
            if (mes < 1 || mes > 12 || anio < 2000)
                return BadRequest();

            var registros = _context.ResumenAsistenciaMensuals
                .Where(r => r.Mes == mes && r.Anio == anio);

            _context.ResumenAsistenciaMensuals.RemoveRange(registros);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Se eliminaron las métricas de {mes}/{anio}.";
            return RedirectToAction(nameof(MesesDisponibles));
        }

        [HttpGet]
        public IActionResult ResumenPorMes(int mes, int anio)
        {
            var resumen = _context.ResumenAsistenciaMensuals
                .Where(r => r.Mes == mes && r.Anio == anio)
                .ToList();

            return View("Resumen", resumen); // Usa la misma vista de resumen que ya tienes
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarArchivo(IFormFile archivo, int mes)
        {
            if (archivo == null || archivo.Length == 0 || mes == 0)
            {
                ViewBag.Error = "Debe seleccionar un archivo y un mes válido.";
                return View("Index");
            }

            var anio = DateTime.Now.Year;

            List<RegistroPonche> registros;

            using (var stream = archivo.OpenReadStream())
            {
                registros = LeerExcel(stream);
            }

            var resumen = _poncheService.CalcularResumen(registros);

            // Guardar en la tabla
            foreach (var r in resumen)
            {
                var existente = await _context.ResumenAsistenciaMensuals.FirstOrDefaultAsync(x =>
                    x.Empleado == r.Nombre.Trim().ToLower() &&
                    x.Mes == mes &&
                    x.Anio == anio);

                if (existente != null)
                {
                    // Actualizar valores existentes
                    existente.Seccion = r.Seccion;
                    existente.DiasTotales = r.TotalDias;
                    existente.DiasTarde = r.DiasTarde;
                    existente.MinTarde = (int)r.MinutosTotalesTarde;
                    existente.DiasAnticipados = r.DiasSalidaAnticipada;
                    existente.MinAnticipados = (int)r.MinutosTotalesAnticipados;
                    existente.HorasTrabajadas = (decimal)r.HorasTrabajadasTotales;
                    existente.PorcCumplimiento = decimal.Parse(r.Cumplimiento.Replace("%", ""), CultureInfo.InvariantCulture);

                    _context.ResumenAsistenciaMensuals.Update(existente);
                }
                else
                {
                    // Crear nuevo
                    var nuevo = new ResumenAsistenciaMensual
                    {
                        Empleado = r.Nombre?.Trim(),
                        Seccion = r.Seccion,
                        Mes = mes,
                        Anio = anio,
                        DiasTotales = r.TotalDias,
                        DiasTarde = r.DiasTarde,
                        MinTarde = (int)r.MinutosTotalesTarde,
                        DiasAnticipados = r.DiasSalidaAnticipada,
                        MinAnticipados = (int)r.MinutosTotalesAnticipados,
                        HorasTrabajadas = (decimal)r.HorasTrabajadasTotales,
                        PorcCumplimiento = decimal.Parse(r.Cumplimiento.Replace("%", ""), CultureInfo.InvariantCulture)
                    };

                    _context.ResumenAsistenciaMensuals.Add(nuevo);
                }
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("ResumenPorMes", new {mes, anio = DateTime.Now.Year });
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
