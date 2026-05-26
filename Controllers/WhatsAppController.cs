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
    public async Task<IActionResult> Enviar([FromBody] WhatsAppRequest cita)
    {
        var mensaje =
            $"Hola {cita.NombreCompleto} 👋\n\n" +
            $"Le recordamos su cita para el {cita.Fecha:dd/MM/yyyy}.\n\n" +
            $"Confirme o cancele:\n\n" +
            $"✅ Confirmar: https://clinica-api-sofi.onrender.com/api/citas/confirmar/{cita.Id}\n\n" +
            $"❌ Cancelar: https://clinica-api-sofi.onrender.com/api/citas/cancelar/{cita.Id}\n\n" +
            $"🔄 Reprogramar:\n" +
            $"https://clinica-api-sofi.onrender.com/api/citas/reprogramar/{cita.Id}\n\n" +
            $"Saludos,\n{cita.Clinica}";

        var success = await _whatsAppService.EnviarAsync(
            cita.Telefono,
            mensaje
        );

        if (!success)
            return BadRequest(new
            {
                ok = false,
                message = "No se pudo enviar WhatsApp"
            });

        return Ok(new
        {
            ok = true,
            message = "WhatsApp enviado"
        });
    }
}

public class WhatsAppRequest
{
    public int Id { get; set; }

    public string NombreCompleto { get; set; } = "";

    public DateTime Fecha { get; set; }

    public string Clinica { get; set; } = "";

    public string Telefono { get; set; } = "";
}