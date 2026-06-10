using System.Net;
using System.Net.Mail;

namespace ClinicaAPI.Interface
{
    public class EmailService : IEmailService
    {
        private readonly string _email = "jsolorzano.fc2018@gmail.com";
        private readonly string _password = "fgcqzlsclxzssokd";

        public async Task EnviarCorreoAsync(string destino, string asunto, string htmlBody)
        {
            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(_email, _password),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Timeout = 30000
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(_email, "sofsystem"),
                Subject = asunto,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mail.To.Add(destino);

            await smtp.SendMailAsync(mail);
        }
    }
}