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

        public WhatsAppSchedulerService(IServiceScopeFactory scopeFactory, IConfiguration config, ILogger<WhatsAppSchedulerService> logger) { _scopeFactory = scopeFactory; _config = config; _logger = logger; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) { _logger.LogInformation("🔥 WhatsAppSchedulerService iniciado"); while (!stoppingToken.IsCancellationRequested) { try { _logger.LogInformation("⏰ Procesando citas..."); await ProcesarCitas(); _logger.LogInformation("✅ Proceso finalizado"); } catch (Exception ex) { _logger.LogError(ex, "❌ Error en scheduler"); } await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); } }

        private async Task ProcesarCitas()
        {
            var clinica = _config["Clinica"]; // opcional si lo usas

            using var connection = new SqlConnection(
                _config.GetConnectionString("EntitiesContext")
            );

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

            foreach (var cita in citas)
            {

                var apiBaseUrl = _config["ApiBaseUrl"];

                var urlConfirmar =
                    $"Cita Confirmada: {apiBaseUrl}/api/citas/confirmar/{cita.Id}";

                var urlCancelar =
                    $"Cita cancelada: {apiBaseUrl}/api/citas/cancelar/{cita.Id}";


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

                await EnviarWhatsApp(cita.Telefono, mensaje);

                await connection.ExecuteAsync(@"
                    UPDATE Citas
                    SET WhatsAppEnviado = 1
                    WHERE Id = @Id
                ", new { cita.Id });
            }
        }

        private async Task EnviarWhatsApp(string telefono, string mensaje)
        {
            var instanceId = _config["UltraMsg:InstanceId"];
            var token = _config["UltraMsg:Token"];

            using var client = new HttpClient();

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

            Console.WriteLine(result);

            response.EnsureSuccessStatusCode();
        }
    }
}