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

                // =========================================================
                // 1. BUSCAR LA CITA
                // =========================================================
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


                // =========================================================
                // 2. VALIDAR QUE LA CITA EXISTA
                // =========================================================
                if (cita == null)
                {
                    _logger.LogWarning(
                        $"❌ No se encontró la cita con ID: {id}"
                    );

                    return NotFound($@"
                <html>
                <head>
                    <meta name='viewport'
                          content='width=device-width, initial-scale=1.0'>
                </head>

                <body style='
                    font-family: Arial;
                    background-color:#f5f5f5;
                    margin:0;
                    padding:20px;
                '>

                    <div style='
                        max-width:400px;
                        margin:60px auto;
                        background:white;
                        padding:30px;
                        border-radius:12px;
                        text-align:center;
                        box-shadow:0 2px 10px rgba(0,0,0,0.1);
                    '>

                        <h1 style='color:#dc2626;'>
                            ❌ Cita no encontrada
                        </h1>

                        <p style='font-size:18px;color:#555;'>
                            No existe una cita con el ID
                            <strong>{id}</strong>.
                        </p>

                    </div>

                </body>
                </html>
            ");
                }


                // =========================================================
                // 3. MOSTRAR INFORMACIÓN EN LOG
                // =========================================================
                _logger.LogInformation("📊 SQL RESULT:");
                _logger.LogInformation($"ID: {cita.Id}");
                _logger.LogInformation($"Doctor: {cita.NombreDoctor}");
                _logger.LogInformation($"TelefonoDoctor: '{cita.TelefonoDoctor}'");
                _logger.LogInformation($"Paciente: {cita.NombreCompleto}");
                _logger.LogInformation($"Fecha: {cita.Fecha}");
                _logger.LogInformation($"Hora: {cita.Hora}");
                _logger.LogInformation($"Estado actual: {cita.Estado}");
                _logger.LogInformation($"Respondida: {cita.Respondida}");


                // =========================================================
                // 4. VALIDAR SI YA FUE RESPONDIDA
                // =========================================================
                if (cita.Respondida)
                {
                    _logger.LogWarning(
                        $"⚠️ La cita {id} ya fue respondida. " +
                        $"Estado actual: {cita.Estado}"
                    );

                    return Content(@"
                <html>

                <head>
                    <meta name='viewport'
                          content='width=device-width, initial-scale=1.0'>
                </head>

                <body style='
                    font-family: Arial, sans-serif;
                    background-color:#f5f5f5;
                    margin:0;
                    padding:20px;
                '>

                    <div style='
                        max-width:400px;
                        margin:60px auto;
                        background:white;
                        padding:30px;
                        border-radius:12px;
                        text-align:center;
                        box-shadow:0 2px 10px rgba(0,0,0,0.1);
                    '>

                        <h1 style='
                            color:#d97706;
                            font-size:45px;
                        '>
                            ⚠️
                        </h1>

                        <h2 style='color:#d97706;'>
                            Acción ya realizada
                        </h2>

                        <p style='
                            font-size:20px;
                            color:#444;
                            margin-top:30px;
                        '>
                            Esta cita ya fue
                            <strong>confirmada</strong>,
                            <strong>cancelada</strong>
                            o <strong>reprogramada</strong>.
                        </p>

                        <p style='
                            font-size:17px;
                            color:#666;
                            margin-top:25px;
                        '>
                            No se puede realizar nuevamente esta acción.
                        </p>

                    </div>

                </body>
                </html>
            ", "text/html; charset=utf-8");
                }


                // =========================================================
                // 5. VALIDAR QUE LA CITA ESTÉ PENDIENTE
                // =========================================================
                if (cita.Estado != "Pendiente")
                {
                    _logger.LogWarning(
                        $"⚠️ Intento de confirmar cita {id} " +
                        $"con estado actual: {cita.Estado}"
                    );

                    return Content($@"
                <html>

                <head>
                    <meta name='viewport'
                          content='width=device-width, initial-scale=1.0'>
                </head>

                <body style='
                    font-family: Arial;
                    background-color:#f5f5f5;
                    margin:0;
                    padding:20px;
                '>

                    <div style='
                        max-width:400px;
                        margin:60px auto;
                        background:white;
                        padding:30px;
                        border-radius:12px;
                        text-align:center;
                        box-shadow:0 2px 10px rgba(0,0,0,0.1);
                    '>

                        <h1 style='color:#d97706;'>
                            ⚠️ Cita no disponible
                        </h1>

                        <p style='
                            font-size:18px;
                            color:#444;
                        '>
                            Esta cita ya no está pendiente.
                        </p>

                        <p style='
                            font-size:18px;
                            color:#666;
                        '>
                            Estado actual:
                            <strong>{cita.Estado}</strong>
                        </p>

                    </div>

                </body>
                </html>
            ", "text/html; charset=utf-8");
                }


                // =========================================================
                // 6. ACTUALIZAR CITA A CONFIRMADA
                // =========================================================
                var filasActualizadas = await connection.ExecuteAsync(@"
            UPDATE Citas
            SET
                Estado = 'Confirmada',
                Respondida = 1,
                FechaConfirmacion = GETDATE()
            WHERE Id = @Id
              AND Estado = 'Pendiente'
              AND Respondida = 0
        ", new { Id = id });


                // =========================================================
                // 7. VERIFICAR QUE REALMENTE SE ACTUALIZÓ
                // =========================================================
                if (filasActualizadas == 0)
                {
                    _logger.LogWarning(
                        $"⚠️ No se actualizó la cita {id}. " +
                        $"Posiblemente ya fue procesada."
                    );

                    return Content(@"
                <html>

                <head>
                    <meta name='viewport'
                          content='width=device-width, initial-scale=1.0'>
                </head>

                <body style='
                    font-family: Arial;
                    background-color:#f5f5f5;
                    margin:0;
                    padding:20px;
                '>

                    <div style='
                        max-width:400px;
                        margin:60px auto;
                        background:white;
                        padding:30px;
                        border-radius:12px;
                        text-align:center;
                        box-shadow:0 2px 10px rgba(0,0,0,0.1);
                    '>

                        <h1 style='color:#d97706;'>
                            ⚠️ Acción no realizada
                        </h1>

                        <p style='font-size:18px;color:#555;'>
                            La cita ya fue procesada o
                            cambió de estado.
                        </p>

                    </div>

                </body>
                </html>
            ", "text/html; charset=utf-8");
                }


                // =========================================================
                // 8. LOG DE CONFIRMACIÓN
                // =========================================================
                _logger.LogInformation(
                    $"✅ Cita {id} actualizada correctamente a CONFIRMADA"
                );


                // =========================================================
                // 9. GENERAR URL PARA NOTIFICAR AL DOCTOR
                // =========================================================
                var mensajeDoctor =
                    $"📢 El paciente {cita.NombreCompleto} " +
                    $"CONFIRMÓ la cita del día " +
                    $"{cita.Fecha:dd/MM/yyyy}";

                var mensajeDoctorUrl =
                    Uri.EscapeDataString(mensajeDoctor);

                var telefonoDoctor =
                    cita.TelefonoDoctor?.Replace("+", "")
                                       .Replace(" ", "")
                                       .Replace("-", "")
                                       .Replace("(", "")
                                       .Replace(")", "");


                // =========================================================
                // 10. RESPUESTA HTML
                // =========================================================
                return Content($@"
            <html>

            <head>

                <meta name='viewport'
                      content='width=device-width, initial-scale=1.0'>

                <title>Cita confirmada</title>

            </head>

            <body style='
                font-family:Arial, sans-serif;
                background-color:#f5f5f5;
                margin:0;
                padding:20px;
            '>

                <div style='
                    max-width:400px;
                    margin:60px auto;
                    background:white;
                    padding:30px;
                    border-radius:12px;
                    text-align:center;
                    box-shadow:0 2px 10px rgba(0,0,0,0.1);
                '>

                    <div style='
                        font-size:16px;
                        color:#555;
                        margin-bottom:20px;
                    '>
                        {cita.Clinica} 🏥
                    </div>


                    <div style='
                        font-size:55px;
                        margin-bottom:10px;
                    '>
                        ✅
                    </div>


                    <h2 style='
                        color:#16a34a;
                        margin-top:10px;
                    '>
                        Cita confirmada
                    </h2>


                    <p style='
                        font-size:18px;
                        color:#333;
                    '>
                        Gracias
                        <strong>{cita.NombreCompleto}</strong>
                    </p>


                    <p style='
                        font-size:16px;
                        color:#555;
                    '>
                        Su cita fue confirmada correctamente.
                    </p>


                    <hr style='margin:25px 0;'>


                    <p style='
                        font-size:16px;
                        color:#333;
                    '>
                        📅 Fecha:
                        <strong>
                            {cita.Fecha:dd/MM/yyyy}
                        </strong>
                    </p>


                    <p style='
                        font-size:16px;
                        color:#333;
                    '>
                        🕐 Hora:
                        <strong>
                            {cita.Hora}
                        </strong>
                    </p>


                    <hr style='margin:25px 0;'>


                    <p style='
                        font-size:16px;
                        color:#333;
                    '>
                        📢 Notificar al doctor
                    </p>


                    {(string.IsNullOrWhiteSpace(telefonoDoctor)
                                ? ""
                                : $@"
                            <a href='https://api.whatsapp.com/send?phone={telefonoDoctor}&text={mensajeDoctorUrl}'
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
                        ")}


                    <hr style='margin:25px 0;'>


                    <p style='
                        font-size:14px;
                        color:#888;
                    '>
                        Puede cerrar esta ventana.
                    </p>

                </div>

            </body>
            </html>
        ", "text/html; charset=utf-8");
            }
            catch (Exception ex)
            {
                // =========================================================
                // ERROR
                // =========================================================

                _logger.LogError(
                    ex,
                    $"❌ Error confirmando la cita {id}"
                );

                return Content($@"
            <html>

            <head>

                <meta name='viewport'
                      content='width=device-width, initial-scale=1.0'>

            </head>

            <body style='
                font-family:Arial;
                background-color:#f5f5f5;
                padding:20px;
            '>

                <div style='
                    max-width:500px;
                    margin:60px auto;
                    background:white;
                    padding:30px;
                    border-radius:12px;
                    box-shadow:0 2px 10px rgba(0,0,0,0.1);
                '>

                    <h2 style='color:#dc2626;'>
                        ❌ Error al confirmar la cita
                    </h2>

                    <p>
                        <strong>Mensaje:</strong>
                        {System.Net.WebUtility.HtmlEncode(ex.Message)}
                    </p>

                    <p>
                        <strong>Detalle:</strong>
                        {System.Net.WebUtility.HtmlEncode(
                                    ex.InnerException?.Message ?? "N/A"
                                )}
                    </p>

                </div>

            </body>
            </html>
        ", "text/html; charset=utf-8");
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
