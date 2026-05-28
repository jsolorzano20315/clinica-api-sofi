using System.Net.Mail;
using System.Net;

namespace ClinicaAPI.Interface
{
    public class EmailService : IEmailService
    {
        public async Task EnviarCorreoAsync(string destino, string asunto, string mensaje)
        {
            var fromEmail = "jsolorzano.fc2018@gmail.com";
            var password = "fgcqzlsclxzssokd";

            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail, "sofsystem"),
                Subject = asunto,
                Body = mensaje,
                IsBodyHtml = true
            };

            mail.To.Add(destino);

            await smtp.SendMailAsync(mail);
        }
    }
}
