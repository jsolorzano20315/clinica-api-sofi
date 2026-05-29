using Resend;

namespace ClinicaAPI.Interface
{
    public class EmailService : IEmailService
    {
        private readonly IResend _resend;

        public EmailService(IResend resend)
        {
            _resend = resend;
        }

        public async Task EnviarCorreoAsync(
            string destino,
            string asunto,
            string mensaje)
        {
            await _resend.EmailSendAsync(new EmailMessage
            {
                From = "sofsystem <onboarding@resend.dev>",
                To = destino,
                Subject = asunto,
                HtmlBody = mensaje
            });
        }
    }
}