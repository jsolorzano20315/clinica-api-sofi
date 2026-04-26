using ClinicaAPI.Data;
using ClinicaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class HistorialController : ControllerBase
    { 
        private readonly ClinicaContext _context;

        public HistorialController(ClinicaContext context) 
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Historial>> Get()
        {
            return await _context.Historiales.ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Post(Historial historial)
        {
            _context.Historiales.Add(historial);
            await _context.SaveChangesAsync();
            return Ok(historial);
        }
    }
}
