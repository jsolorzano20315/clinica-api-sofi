using ClinicaAPI.DTOs;
using ClinicaAPI.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WhatsAppController : ControllerBase
{
    private readonly WhatsAppService _whatsAppService;

    public WhatsAppController(WhatsAppService whatsAppService)
    {
        _whatsAppService = whatsAppService;
    }

    [HttpPost("EnviarWhatsApp")]
    public async Task<IActionResult> EnviarWhatsApp([FromBody] WhatsAppDTO dto)
    {
        await _whatsAppService.EnviarAsync(dto.Telefono, dto.Mensaje);
        return Ok("Mensaje enviado");
    }

    [HttpPost("enviar")]
    public IActionResult Enviar([FromBody] WhatsAppRequest cita)
        {
            if (cita == null)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "No se recibió información de la cita."
                });
            }

            if (string.IsNullOrWhiteSpace(cita.Telefono))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "La cita no tiene teléfono."
                });
            }

            if (cita.Id <= 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "La cita no tiene un ID válido."
                });
            }

        // Limpiar teléfono: solo números
        var telefono = new string(
            cita.Telefono
                .Where(char.IsDigit)
                .ToArray()
            );

        if (telefono.Length == 8)
        {
            telefono = "504" + telefono;
        }

        if (string.IsNullOrWhiteSpace(telefono))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El número de teléfono no es válido."
                });
            }

        // Mensaje que recibirá el paciente
        var mensaje =
                 $"Hola {cita.NombreCompleto}\n\n" +
                 $"Le recordamos su cita para el {cita.Fecha:dd/MM/yyyy}.\n\n" +
                 $"Agradeceremos que nos confirme su asistencia.\n\n" +
                 $"Quedamos atentos a su confirmación.\n\n" +
                 $"Saludos cordiales,\n" +
                 $"{cita.Clinica}";

        // Crear URL de WhatsApp
        var urlWhatsApp =
                $"https://wa.me/{telefono}?text={Uri.EscapeDataString(mensaje)}";

            return Ok(new
            {
                ok = true,
                message = "URL de WhatsApp generada correctamente.",
                url = urlWhatsApp
            });
        }


    //[HttpPost("enviar")]
    //public async Task<IActionResult> Enviar([FromBody] WhatsAppRequest cita)
    //{
    //    var mensaje =
    //        $"Hola {cita.NombreCompleto} 👋\n\n" +
    //        $"Le recordamos su cita para el {cita.Fecha:dd/MM/yyyy}.\n\n" +
    //        $"Confirme o cancele:\n\n" +
    //        $"✅ Confirmar: https://clinica-api-sofi.onrender.com/api/citas/confirmar/{cita.Id}\n\n" +
    //        $"❌ Cancelar: https://clinica-api-sofi.onrender.com/api/citas/cancelar/{cita.Id}\n\n" +
    //        $"🔄 Reprogramar:\n" +
    //        $"https://clinica-api-sofi.onrender.com/api/citas/reprogramar/{cita.Id}\n\n" +
    //        $"Saludos,\n{cita.Clinica}";

    //    var success = await _whatsAppService.EnviarAsync(
    //        cita.Telefono,
    //        mensaje
    //    );

    //    if (!success)
    //        return BadRequest(new
    //        {
    //            ok = false,
    //            message = "No se pudo enviar WhatsApp"
    //        });

    //    return Ok(new
    //    {
    //        ok = true,
    //        message = "WhatsApp enviado"
    //    });
    //}
}

public class WhatsAppRequest
{
    public int Id { get; set; }

    public string NombreCompleto { get; set; } = "";

    public DateTime Fecha { get; set; }

    public string Clinica { get; set; } = "";

    public string Telefono { get; set; } = "";
}