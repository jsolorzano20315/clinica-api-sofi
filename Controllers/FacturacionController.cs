using ClinicaAPI.DTOs;
using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
     public class FacturacionController : ControllerBase
    {
        private readonly AuthService _authService; 
        public FacturacionController(IConfiguration configuration, AuthService authService)
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost]
        [Route("GuardarFactura")]
        public async Task<IActionResult> GuardarFactura([FromBody] Facturas model)
        {
            try
            {
                var query = new StringBuilder();

                query.AppendLine(StaticResources.QueryCrearFacturacion);

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@PacienteId", model.PacienteId);
                parameters.Add("@Fecha", model.Fecha);
                parameters.Add("@Total", model.Total);
                parameters.Add("@Clinica", model.Clinica);

                using var connection = new SqlConnection(
                    Configuration.GetConnectionString("EntitiesContext")
                );

                await connection.ExecuteAsync(
                    query.ToString(),
                    parameters
                );

                return Ok(new
                {
                    success = true,
                    message = "Factura guardada correctamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet()]
        [Route("ListaFacturas/{clinica}")]
        public async Task<IActionResult> ListaFacturas(string clinica) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<FacturaDto> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearFacturacion);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Clinica", clinica);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<FacturaDto>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<FacturaDto>());

        }
    }
}