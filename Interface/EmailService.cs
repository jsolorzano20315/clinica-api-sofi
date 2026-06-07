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
            try
            {
                Console.WriteLine("=================================");
                Console.WriteLine("USANDO SMTP GMAIL");
                Console.WriteLine($"DESTINO: {destino}");
                Console.WriteLine("=================================");

                ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls12;

                var fromEmail =
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
                                fromEmail,
                                password
                            ),
                        Timeout = 30000
                    };

                using var mail =
                    new MailMessage
                    {
                        From = new MailAddress(
                            fromEmail,
                            "sofsystem"
                        ),
                        Subject = asunto,
                        Body = mensaje,
                        IsBodyHtml = true
                    };

                mail.To.Add(destino);

                Console.WriteLine("Enviando correo...");

                await smtp.SendMailAsync(mail);

                Console.WriteLine("✅ CORREO ENVIADO");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR AL ENVIAR CORREO");
                Console.WriteLine(ex.ToString());

                throw;
            }
        }
    }
}