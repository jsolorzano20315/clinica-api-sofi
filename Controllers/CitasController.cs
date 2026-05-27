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
        private readonly NotificacionesService _notificacionesService;

        public CitasController(IConfiguration configuration, AuthService authService, 
                               WhatsAppService whatsAppService, ILogger<WhatsAppSchedulerService> logger, 
                               NotificacionesService notificacionesService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _whatsAppService = whatsAppService;
            _logger = logger;
            _notificacionesService = notificacionesService;
        }


        public IConfiguration Configuration { get; }

        [HttpPost("GuardarCitas")]
        public async Task<IActionResult> GuardarCitas([FromBody] Cita model)
        {

            var userName = User.Identity?.Name ?? "Anonimo";

           // List<Cita> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearCitas);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@PacienteId", model.PacienteId);
            parameters.Add("@DoctorId", model.DoctorId);
            parameters.Add("@Fecha", model.Fecha);
            parameters.Add("@Hora", model.Hora ?? "");
            parameters.Add("@Motivo", model.Motivo);
            parameters.Add("@Tipo", model.Tipo);
            parameters.Add("@Telefono", model.Telefono);
            parameters.Add("@Estado", model.Estado);
            parameters.Add("@Clinica", model.Clinica); 

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            var id = await connection.ExecuteScalarAsync<int>(
                query.ToString(),
                parameters
            );

            return Ok(new
            {
                id = id
            });
        }

        [HttpPut("EditarCitas")]
        public async Task<IActionResult> EditarCitas([FromBody] Cita model)
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Cita> result;
            var query = new StringBuilder();
            query.AppendLine(StaticResources.QueryModificarCitas);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", model.Id);
            parameters.Add("@Fecha", model.Fecha);
            parameters.Add("@Hora", model.Hora ?? "");
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
                            a.Hora,
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

                    _logger.LogInformation($"📊 SQL RESULT:");
                    _logger.LogInformation($"Doctor: {cita.NombreDoctor}");
                    _logger.LogInformation($"TelefonoDoctor: '{cita.TelefonoDoctor}'");
                    _logger.LogInformation($"Paciente: {cita.NombreCompleto}");
                    _logger.LogInformation($"Fecha: {cita.Fecha}");

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
                            Esta cita ya fue <b>confirmada</b>,
                                              <b>cancelada</b> o <b>reprogramada</b>.
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

                    <p style='font-size:16px; color:#333;'>
                        📢 Notificar al doctor:
                    </p>

                    <a href='https://api.whatsapp.com/send?phone={cita.TelefonoDoctor}&text=📢%20El%20paciente%20{cita.NombreCompleto}%20CONFIRMÓ%20la%20cita%20del%20día%20{cita.Fecha:dd/MM/yyyy}'
                       style='
                           display:inline-block;
                           background-color:#25D366;
                           color:white;
                           padding:12px 18px;
                           border-radius:8px;
                           text-decoration:none;
                           font-weight:bold;
                           margin-top:10px;
                       '
                       target='_blank'>
                       💬 Enviar WhatsApp al doctor
                    </a>

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
                            Esta cita ya fue <b>confirmada</b>,
                                             <b>cancelada</b> o <b>reprogramada</b>.
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

                        <p style='font-size:16px; color:#333;'>
                            📢 Notificar al doctor:
                        </p>

                        <a href='https://api.whatsapp.com/send?phone={cita.TelefonoDoctor}&text=📢%20El%20paciente%20{cita.NombreCompleto}%20CANCELO%20la%20cita%20del%20día%20{cita.Fecha:dd/MM/yyyy}'
                           style='
                               display:inline-block;
                               background-color:#25D366;
                               color:white;
                               padding:12px 18px;
                               border-radius:8px;
                               text-decoration:none;
                               font-weight:bold;
                               margin-top:10px;
                           '
                           target='_blank'>
                           💬 Enviar WhatsApp al doctor
                        </a>

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

        [HttpGet("reprogramar/{id}")]
        public async Task<IActionResult> Reprogramar(int id)
        {
            _logger.LogInformation($"🔄 Entró endpoint reprogramar con ID: {id}");

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

                _logger.LogInformation($"📊 SQL RESULT:");
                _logger.LogInformation($"Doctor: {cita.NombreDoctor}");
                _logger.LogInformation($"TelefonoDoctor: '{cita.TelefonoDoctor}'");
                _logger.LogInformation($"Paciente: {cita.NombreCompleto}");
                _logger.LogInformation($"Fecha: {cita.Fecha}");

                // 🔥 Validar si ya respondió
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
                    Esta cita ya fue <b>confirmada</b>,
                                     <b>cancelada</b> o <b>reprogramada</b>.
                </p>

                <p style='font-size: 20px; color: #666; margin-top: 35px;'>
                    No se puede realizar nuevamente esta acción.
                </p>
            </div>
            ", "text/html; charset=utf-8");
                }

                // 🔥 Marcar como pendiente de reprogramación
                await connection.ExecuteAsync(@"
            UPDATE Citas
            SET Estado = 'ReprogramacionPendiente',
                Respondida = 1
            WHERE Id = @Id
        ", new { Id = id });

                _logger.LogInformation($"✅ Cita marcada para reprogramación: {id}");

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

                <div style='font-size:10px;'>
                    {cita.Clinica} 🏥
                </div>

                <h2 style='color:#f57c00;'>
                    🔄 Solicitud enviada
                </h2>

                <p style='font-size:18px;'>
                    Gracias <strong>{cita.NombreCompleto}</strong>
                </p>

                <p style='color:#555;'>
                    Hemos recibido su solicitud de reprogramación.
                </p>

                <p style='color:#555;'>
                    Se le comunicará los cupos disponibles
                    para asignar una nueva cita.
                </p>

                <hr style='margin:25px 0;'>

                <p style='font-size:16px; color:#333;'>
                    📢 Notificar al doctor:
                </p>

                <a href='https://api.whatsapp.com/send?phone={cita.TelefonoDoctor}&text=📢%20El%20paciente%20{cita.NombreCompleto}%20SOLICITÓ%20REPROGRAMAR%20la%20cita%20del%20día%20{cita.Fecha:dd/MM/yyyy}'
                   style='
                       display:inline-block;
                       background-color:#25D366;
                       color:white;
                       padding:12px 18px;
                       border-radius:8px;
                       text-decoration:none;
                       font-weight:bold;
                       margin-top:10px;
                   '
                   target='_blank'>
                   💬 Enviar WhatsApp al doctor
                </a>

                <hr style='margin:25px 0;'>

            </div>

        </body>
        </html>
        ", "text/html; charset=utf-8");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en endpoint reprogramar");

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
