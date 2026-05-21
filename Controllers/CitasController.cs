using ClinicaAPI.Data;
using ClinicaAPI.DTOs;
using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Text;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitasController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly WhatsAppService _whatsAppService;
        private readonly ILogger<WhatsAppSchedulerService> _logger;

        public CitasController(IConfiguration configuration, AuthService authService, WhatsAppService whatsAppService, ILogger<WhatsAppSchedulerService> logger) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _whatsAppService = whatsAppService;
            _logger = logger;
        }


        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarCitas")]
        public async Task<IActionResult> GuardarCitas(Cita model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Cita> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearCitas);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@PacienteId", model.PacienteId);
            parameters.Add("@DoctorId", model.DoctorId);
            parameters.Add("@Fecha", model.Fecha);
            parameters.Add("@Motivo", model.Motivo);
            parameters.Add("@Tipo", model.Tipo);
            parameters.Add("@Telefono", model.Telefono);
            parameters.Add("@Estado", model.Estado);
            parameters.Add("@Clinica", model.Clinica); 

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Cita>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Cita>());
        }

        [HttpPut()]
        [Route("EditarCitas/{id}")]
        public async Task<IActionResult> EditarCitas(Cita model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Cita> result;
            var query = new StringBuilder();
            query.AppendLine(StaticResources.QueryModificarCitas);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", model.Id);
            parameters.Add("@Estado", model.Estado);
            parameters.Add("@Tipo", model.Tipo);
            parameters.Add("@Motivo", model.Motivo);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Cita>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Cita>());
        }

        [HttpDelete()]
        [Route("EliminarCitas/{id}")]
        public async Task<IActionResult> EliminarCitas(int id)
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Cita> result;
            var query = new StringBuilder();
            query.AppendLine(StaticResources.QueryEliminarCitas);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", id);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Cita>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Cita>());
        }


        [HttpGet()]
        [Route("ListaCitas/{clinica}")]
        public async Task<IActionResult> ListaCitas(string clinica)
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<CitaDto> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaCitas);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Clinica", clinica);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<CitaDto>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<CitaDto>());

        }

        [HttpGet()]
        [Route("ListaReporteCitas/{clinica}/{fechaInicio}/{fechaFin}")]
        public async Task<IActionResult> ListaReporteCitas(string clinica, DateTime fechaInicio, DateTime fechaFin)  
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            fechaInicio = fechaInicio.Date;
            fechaFin = fechaFin.Date.AddDays(1).AddTicks(-1);

            List<TotalCitasDto> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaCitasFecha);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Clinica", clinica);
            parameters.Add("@FechaInicio", fechaInicio);
            parameters.Add("@FechaFin", fechaFin);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<TotalCitasDto>(query.ToString(), parameters)).ToList();

          return Ok(result ?? new List<TotalCitasDto>());

        } 

        [HttpGet]
        [Route("TotalCitasConfi/{clinica}")]
        public async Task<IActionResult> TotalCitasConf(string clinica)
        {
            var query = new StringBuilder();
            query.AppendLine(StaticResources.QueryTotalCitasConfirmado);

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Clinica", clinica);

            using var connection = new SqlConnection(Configuration.GetConnectionString("EntitiesContext"));

            var result = await connection.QueryFirstOrDefaultAsync<int>(
                query.ToString(),
                parameters
            );

            return Ok(result);
        }

        [HttpGet()]
        [Route("TotalCitasPend/{clinica}")]
        public async Task<IActionResult> TotalCitasPend(string clinica) 
        {

            var query = new StringBuilder();
            query.AppendLine(StaticResources.QueryTotalCitasPendiente);

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Clinica", clinica);

            using var connection = new SqlConnection(Configuration.GetConnectionString("EntitiesContext"));

            var result = await connection.QueryFirstOrDefaultAsync<int>(
                query.ToString(),
                parameters
            );

            return Ok(result);

        }

        [HttpGet()]
        [Route("TotalCitasCance/{clinica}")]
        public async Task<IActionResult> TotalCitasCance(string clinica) 
        {

            var query = new StringBuilder();
            query.AppendLine(StaticResources.QueryTotalCitasCanceladas);

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Clinica", clinica);

            using var connection = new SqlConnection(Configuration.GetConnectionString("EntitiesContext"));

            var result = await connection.QueryFirstOrDefaultAsync<int>(
                query.ToString(),
                parameters
            );

            return Ok(result);

        }


        [HttpGet("confirmar/{id}")]
        public async Task<IActionResult> confirmar(int id)
        {
            _logger.LogInformation($"🔥 Entró endpoint confirmar con ID: {id}");
            try
            {
                using var connection = new SqlConnection(
                    Configuration.GetConnectionString("EntitiesContext")
                );

                await connection.OpenAsync();

                var cita = await connection.QueryFirstOrDefaultAsync<CitaDto>(@"
                        SELECT
                            a.Id,
                            a.Fecha,
                            a.Estado,
                            b.Telefono,
                            c.Telefono AS TelefonoDoctor,
                            c.Nombre AS NombreDoctor,
                            a.Clinica,
                            a.Respondida,
                            CONCAT(b.Nombre, ' ', b.Apellido) AS NombreCompleto
                        FROM Citas a
                        INNER JOIN Paciente b
                            ON a.PacienteId = b.Id
                        INNER JOIN Doctor c
                            ON a.DoctorId = c.Id
                        WHERE a.Id = @Id
                    ", new { Id = id });

                if (cita == null)
                    return NotFound("Cita no encontrada");

                if (cita.Respondida)
                {
                    return Content(@"
                    <div style='
                        font-family: Arial, sans-serif;
                        text-align: center;
                        margin-top: 60px;
                        padding: 30px;
                    '>
                        <h1 style='color: #d97706; font-size: 55px;'>
                            ⚠️ Acción ya realizada
                        </h1>

                        <p style='font-size: 24px; color: #444; margin-top: 45px;'>
                            Esta cita ya fue <b>confirmada</b> o <b>cancelada</b> anteriormente.
                        </p>

                        <p style='font-size: 20px; color: #666; margin-top: 35px;'>
                            No se puede realizar nuevamente esta acción.
                        </p>
                    </div>
                ", "text/html; charset=utf-8");
                }

                await connection.ExecuteAsync(@"
                    UPDATE Citas
                    SET Estado = 'Confirmada',
                        Respondida = 1,
                        FechaConfirmacion = GETDATE()
                    WHERE Id = @Id
                 ", new { Id = id });

                _logger.LogInformation("📨 Preparando envío al doctor");

                _logger.LogInformation($"📞 TelefonoDoctor: {cita.TelefonoDoctor}");
                _logger.LogInformation($"👤 Paciente: {cita.NombreCompleto}");
                _logger.LogInformation($"📅 Fecha: {cita.Fecha}");

                if (string.IsNullOrWhiteSpace(cita.TelefonoDoctor))
                {
                    _logger.LogWarning("⚠️ No se envía WhatsApp: Doctor sin teléfono");
                }
                else
                {
                    var mensajeDoctor =
                        $"📢 El paciente {cita.NombreCompleto} " +
                        $"CONFIRMÓ la cita del día {cita.Fecha:dd/MM/yyyy HH:mm}.";

                    _logger.LogInformation($"📨 Mensaje doctor: {mensajeDoctor}");
                    _logger.LogInformation("🚀 INICIANDO ENVÍO WHATSAPP AL DOCTOR");

                    bool enviado = false;

                    try
                    {
                        enviado = await _whatsAppService.EnviarAsync(
                            cita.TelefonoDoctor,
                            mensajeDoctor
                        );

                        _logger.LogInformation("✅ ENVÍO TERMINADO");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ ERROR enviando WhatsApp al doctor");
                    }

                    _logger.LogInformation($"📲 Resultado final envío doctor: {enviado}");
                }

                return Content($@"
                <html>
                <head>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                </head>

                <body style='
                    font-family: Arial;
                    background-color:#f5f5f5;
                    margin:0;
                    padding:20px;
                '>

                    <div style='
                        max-width:400px;
                        margin:auto;
                        background:white;
                        padding:30px;
                        border-radius:12px;
                        text-align:center;
                        box-shadow:0 2px 10px rgba(0,0,0,0.1);
                    '>
                        <div style='font-size:10px;'>{cita.Clinica} 🏥</div>

                        <h2 style='color:#d32f2f;'>
                            ✅ Cita confirmada
                        </h2>

                        <p style='font-size:18px;'>
                            Gracias <strong>{cita.NombreCompleto}</strong>
                        </p>

                        <p style='color:#555;'>
                            Su cita fue confirmada correctamente.
                        </p>

                        <hr style='margin:25px 0;'>
                    </div>

                </body>
                </html>
                ", "text/html; charset=utf-8");
            }
            catch (Exception ex)
            {
                return Content($@"
            ERROR:
            {ex.Message}

            INNER:
            {ex.InnerException?.Message}
        ");
            }
        }

        [HttpGet("cancelar/{id}")]
        public async Task<IActionResult> cancelar(int id) 
        {
            try
            {
                using var connection = new SqlConnection(
                    Configuration.GetConnectionString("EntitiesContext")
                );

                await connection.OpenAsync();

                var cita = await connection.QueryFirstOrDefaultAsync<CitaDto>(@"
                SELECT
                    a.Id,
                    a.Fecha,
                    a.Estado,
                    b.Telefono,
                    c.Telefono AS TelefonoDoctor,
                    c.Nombre AS NombreDoctor,
                    a.Clinica,
                    a.Respondida,
                    CONCAT(b.Nombre, ' ', b.Apellido) AS NombreCompleto
                FROM Citas a
                INNER JOIN Paciente b
                    ON a.PacienteId = b.Id
                INNER JOIN Doctor c
                    ON a.DoctorId = c.Id
                WHERE a.Id = @Id
            ", new { Id = id });

                if (cita == null)
                    return NotFound("Cita no encontrada");

                if (cita.Respondida)
                {
                    return Content(@"
                    <div style='
                        font-family: Arial, sans-serif;
                        text-align: center;
                        margin-top: 60px;
                        padding: 30px;
                    '>
                        <h1 style='color: #d97706; font-size: 55px;'>
                            ⚠️ Acción ya realizada
                        </h1>

                        <p style='font-size: 24px; color: #444; margin-top: 45px;'>
                            Esta cita ya fue <b>confirmada</b> o <b>cancelada</b> anteriormente.
                        </p>

                        <p style='font-size: 20px; color: #666; margin-top: 35px;'>
                            No se puede realizar nuevamente esta acción.
                        </p>
                    </div>
                ", "text/html; charset=utf-8");
                }

                await connection.ExecuteAsync(@"
                    UPDATE Citas
                    SET Estado = 'Cancelada',
                        Respondida = 1,
                        FechaConfirmacion = GETDATE()
                    WHERE Id = @Id
                 ", new { Id = id });

                // ✅ MENSAJE AL DOCTOR
                var mensajeDoctor =
                    $"📢 El paciente {cita.NombreCompleto} " +
                    $"CANCELO la cita del día {cita.Fecha:dd/MM/yyyy HH:mm}.";

                await _whatsAppService.EnviarAsync(cita.TelefonoDoctor, mensajeDoctor);

                return Content($@"
                <html>
                <head>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                </head>

                <body style='
                    font-family: Arial;
                    background-color:#f5f5f5;
                    margin:0;
                    padding:20px;
                '>

                    <div style='
                        max-width:400px;
                        margin:auto;
                        background:white;
                        padding:30px;
                        border-radius:12px;
                        text-align:center;
                        box-shadow:0 2px 10px rgba(0,0,0,0.1);
                    '>
                        <div style='font-size:10px;'>{cita.Clinica} 🏥</div>

                        <h2 style='color:#d32f2f;'>
                            ❌ Cita cancelada
                        </h2>

                        <p style='font-size:18px;'>
                            Gracias <strong>{cita.NombreCompleto}</strong>
                        </p>

                        <p style='color:#555;'>
                            Su cita fue cancelada correctamente.
                        </p>

                        <hr style='margin:25px 0;'>
                    </div>

                </body>
                </html>
                ", "text/html; charset=utf-8");
            }
            catch (Exception ex)
            {
                return Content($@"
            ERROR:
            {ex.Message}

            INNER:
            {ex.InnerException?.Message}
        ");
            }
        }
    }
}
