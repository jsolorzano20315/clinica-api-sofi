using ClinicaAPI.DTOs;

namespace ClinicaAPI.Services
{
    public class NotificacionesService
    {
        private readonly ILogger<NotificacionesService> _logger;
        private readonly WhatsAppService _whatsAppService;

        public NotificacionesService(
            ILogger<NotificacionesService> logger,
            WhatsAppService whatsAppService)
        {
            _logger = logger;
            _whatsAppService = whatsAppService;
        }

        public async Task NotificarDoctorConfirmacion(CitaDto cita)
        {
            if (string.IsNullOrWhiteSpace(cita.TelefonoDoctor))
            {
                _logger.LogWarning("⚠️ Doctor sin teléfono");
                return;
            }

            var mensajeDoctor =
                $"📢 El paciente {cita.NombreCompleto} " +
                $"CONFIRMÓ la cita del día {cita.Fecha:dd/MM/yyyy HH:mm}.";

            _logger.LogInformation($"📨 Enviando WhatsApp doctor: {cita.TelefonoDoctor}");

            try
            {
                var enviado = await _whatsAppService.EnviarAsync(
                    cita.TelefonoDoctor,
                    mensajeDoctor
                );

                _logger.LogInformation($"📲 WhatsApp doctor enviado: {enviado}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error enviando WhatsApp al doctor");
            }
        }
    }
}