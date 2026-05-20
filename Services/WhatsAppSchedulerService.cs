using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dapper;
using Microsoft.Data.SqlClient;
using ClinicaAPI.Models;
using ClinicaAPI.DTOs;
using static System.Net.WebRequestMethods;
using System;

namespace ClinicaAPI.Services
{
    public class WhatsAppSchedulerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<WhatsAppSchedulerService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public WhatsAppSchedulerService(
            IServiceScopeFactory scopeFactory,
            IConfiguration config,
            ILogger<WhatsAppSchedulerService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _scopeFactory = scopeFactory;
            _config = config;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) { _logger.LogInformation("🔥 WhatsAppSchedulerService iniciado"); while (!stoppingToken.IsCancellationRequested) { try { _logger.LogInformation("⏰ Procesando citas..."); await ProcesarCitas(); _logger.LogInformation("✅ Proceso finalizado"); } catch (Exception ex) { _logger.LogError(ex, "❌ Error en scheduler"); } await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); } }

        private async Task ProcesarCitas()
        {
            var clinica = _config["Clinica"]; // opcional si lo usas

            using var connection = new SqlConnection(
                _config.GetConnectionString("EntitiesContext")
            );

            _logger.LogInformation("🔌 Abriendo conexión SQL...");

            await connection.OpenAsync();

            _logger.LogInformation("✅ SQL conectado");

            var hoy = DateTime.Today;

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
                WHERE CAST(a.Fecha AS DATE) = @Hoy
                  AND a.Estado = 'Pendiente'
                  AND a.WhatsAppEnviado = 0
            ", new { Hoy = hoy })).ToList();

            _logger.LogInformation($"📌 Citas encontradas: {citas.Count}");

            foreach (var cita in citas)
            {
                _logger.LogInformation(
                    $"📨 Enviando mensaje a {cita.NombreCompleto} - {cita.Telefono}"
                );
                var apiBaseUrl = _config["ApiBaseUrl"] ?? "https://clinica-api-sofi.onrender.com";

                var urlConfirmar =
                     $"✅{apiBaseUrl}/api/citas/confirmar/{cita.Id}";

                var urlCancelar =
                    $"❌{apiBaseUrl}/api/citas/cancelar/{cita.Id}";

                var urlWhatsAppConfirmar =
                    $"https://wa.me/{cita.TelefonoDoctor.Replace("+", "")}?text={Uri.EscapeDataString(urlConfirmar)}";

                var urlWhatsAppCancelar =
                      $"https://wa.me/{cita.TelefonoDoctor.Replace("+", "")}?text={Uri.EscapeDataString(urlCancelar)}";

                var mensaje =
                    $"Hola {cita.NombreCompleto} 👋\n\n" +
                    $"Le recordamos su cita programada para el día {cita.Fecha:dd/MM/yyyy}.\n\n" +
                    $"Por favor confirme o cancele su asistencia presionando uno de los siguientes enlaces:\n\n" +
                    $"✅ Confirmar:{urlWhatsAppConfirmar}\n\n" +
                    $"❌ Cancelar:{urlWhatsAppCancelar}\n\n" +
                    $"Saludos cordiales,\n{cita.Clinica}.";

                var success = await EnviarWhatsApp(cita.Telefono, mensaje);

                if (success)
                {
                    await connection.ExecuteAsync(@"
                        UPDATE Citas
                        SET WhatsAppEnviado = 1
                        WHERE Id = @Id
                    ", new { cita.Id });
                }
            }
        }
        private async Task<bool> EnviarWhatsApp(string telefono, string mensaje)
        {
            try
            {
                var instanceId = _config["UltraMsg:InstanceId"];
                var token = _config["UltraMsg:Token"];

                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(30);

                var body = new Dictionary<string, string>
                    {
                        { "token", token },
                        { "to", telefono.Replace("+", "") },
                        { "body", mensaje }
                    };

                var response = await client.PostAsync(
                 $"https://api.ultramsg.com/{instanceId}/messages/chat",
                     new FormUrlEncodedContent(body)
                 );

                var result = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"📲 StatusCode: {response.StatusCode}");
                _logger.LogInformation($"📲 UltraMsg Response: {result}");

                return response.IsSuccessStatusCode;
            
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error enviando WhatsApp");
                return false;
            }
        }
    }
}