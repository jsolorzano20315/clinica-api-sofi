using ClinicaAPI.DTOs;
using ClinicaAPI.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WhatsAppController : ControllerBase
{
    private readonly WhatsAppService _service;

    public WhatsAppController(WhatsAppService service)
    {
        _service = service;
    }

    [HttpPost("EnviarWhatsApp")]
    public async Task<IActionResult> EnviarWhatsApp([FromBody] WhatsAppDTO dto)
    {
        await _service.EnviarAsync(dto.Telefono, dto.Mensaje);
        return Ok("Mensaje enviado");
    }
}