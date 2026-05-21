using ClinicaAPI.DTOs;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ClinicaAPI.Services
{
    public class WhatsAppSchedulerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<WhatsAppSchedulerService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly WhatsAppService _whatsAppService;

        public WhatsAppSchedulerService(
            IServiceScopeFactory scopeFactory,
            IConfiguration config,
            ILogger<WhatsAppSchedulerService> logger,
            IHttpClientFactory httpClientFactory,
            WhatsAppService whatsAppService)
        {
            _scopeFactory = scopeFactory;
            _config = config;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _whatsAppService = whatsAppService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🔥 WhatsAppSchedulerService iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("⏰ Procesando citas...");
                    await ProcesarCitas();
                    _logger.LogInformation("✅ Proceso finalizado");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error en scheduler");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ProcesarCitas()
        {
            using var connection = new SqlConnection(
                _config.GetConnectionString("EntitiesContext"));

            _logger.LogInformation("🔌 Abriendo conexión SQL...");
            await connection.OpenAsync();
            _logger.LogInformation("✅ SQL conectado");

            // 🔥 FIX IMPORTANTE: usar rango de fechas (evita problemas de timezone)
            var hoy = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                DateTime.UtcNow,
                "Central America Standard Time"
            ).Date;
         

            var inicio = hoy;
            var fin = inicio.AddDays(1);

            _logger.LogInformation($"📅 Rango: {inicio} - {fin}");

            var citas = (await connection.QueryAsync<CitaDto>(@"
                SELECT  a.Id,
                        a.PacienteId,
                        a.Fecha,
                        a.Motivo,
                        a.Tipo,
                        a.Estado,
                        b.Telefono,
                        c.Telefono as TelefonoDoctor,
                        c.Nombre AS NombreDoctor,
                        a.DoctorId,
                        CONCAT(b.Nombre, ' ', b.Apellido) AS NombreCompleto,
                        a.Clinica
                FROM Citas a
                INNER JOIN Paciente b ON a.PacienteId = b.Id
                INNER JOIN Doctor c ON a.DoctorId = c.Id
                WHERE a.Fecha >= @Inicio
                  AND a.Fecha < @Fin
                  AND a.Estado = 'Pendiente'
                  AND a.WhatsAppEnviado = 0
            ", new { Inicio = inicio, Fin = fin })).ToList();

            _logger.LogInformation($"📌 Citas encontradas: {citas.Count}");

            foreach (var cita in citas)
            {
                try
                {
                    _logger.LogInformation($"📨 Enviando a {cita.NombreCompleto} ({cita.Telefono})");

                    var mensaje =
                        $"Hola {cita.NombreCompleto} 👋\n\n" +
                        $"Le recordamos su cita para el {cita.Fecha:dd/MM/yyyy}.\n\n" +
                        $"Confirme o cancele:\n\n" +
                        $"✅ Confirmar: https://clinica-api-sofi.onrender.com/api/citas/confirmar/{cita.Id}\n\n" +
                        $"❌ Cancelar: https://clinica-api-sofi.onrender.com/api/citas/cancelar/{cita.Id}\n\n" +
                        $"Saludos,\n{cita.Clinica}";

                         var success = await _whatsAppService.EnviarAsync(cita.Telefono,mensaje);

                    if (success)
                    {
                        await connection.ExecuteAsync(@"
                            UPDATE Citas
                            SET WhatsAppEnviado = 1
                            WHERE Id = @Id
                        ", new { cita.Id });

                        _logger.LogInformation($"✅ Marcada como enviada: {cita.Id}");
                    }
                    else
                    {
                        _logger.LogWarning($"⚠️ No se envió WhatsApp a {cita.Id}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"❌ Error procesando cita {cita.Id}");
                }
            }
        }     
    }
}