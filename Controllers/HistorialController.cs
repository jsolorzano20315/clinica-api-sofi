using System.Text;
using ClinicaAPI.Data;
using ClinicaAPI.DTOs;
using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class HistorialController : ControllerBase
    {
        private readonly AuthService _authService;
        public HistorialController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpGet()]
        [Route("HistorialClinico/{clinica}")]
        public async Task<IActionResult> HistorialClinico(string clinica) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<HistorialClinicoDto> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryVerHistorialClinico);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Clinica", clinica);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<HistorialClinicoDto>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<HistorialClinicoDto>());

        }

    }
}

