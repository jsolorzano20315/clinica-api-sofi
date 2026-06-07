using System.Net;
using System.Net.Mail;

namespace ClinicaAPI.Interface
{
    public class EmailService : IEmailService
    {
        public async Task EnviarCorreoAsync(
            string destino,
            string asunto,
            string mensaje)
        {
            Console.WriteLine("=================================");
            Console.WriteLine("USANDO SMTP GMAIL");
            Console.WriteLine("=================================");
            // TLS moderno
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

            var SenderEmail =
                "jsolorzano.fc2018@gmail.com";

            var password =
                "fgcqzlsclxzssokd";

            using var smtp =
                new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod =
                        SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials =
                        new NetworkCredential(
                            SenderEmail,
                            password
                        ),
                    Timeout = 30000
                };

            using var mail =
                new MailMessage
                {
                    From = new MailAddress(
                        SenderEmail,
                        "sofsystem"
                    ),
                    Subject = asunto,
                    Body = mensaje,
                    IsBodyHtml = true
                };

            mail.To.Add(destino);

            try
            {
                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("===== SMTP ERROR =====");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                if (ex.InnerException != null)
                {
                    Console.WriteLine("===== INNER =====");
                    Console.WriteLine(ex.InnerException.Message);
                }

                throw;
            }
        }
    }
}