using ClinicaAPI.Data;
using ClinicaAPI.DTOs;
using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctoresController : ControllerBase 
    {
        private readonly AuthService _authService;
        public DoctoresController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarDoctores")]
        public async Task<IActionResult> GuardarDoctores(Doctor model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Doctor> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaDoctores);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Nombre", model.Nombre);
            parameters.Add("@EspecialidadId", model.EspecialidadId);
            parameters.Add("@Telefono", model.Telefono);
            parameters.Add("@Email", model.Email);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Doctor>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Doctor>());
        }

        [HttpGet()]
        [Route("ListaDoctores/{email}")]
        public async Task<IActionResult> ListaDoctores(string email)  
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<DoctoresDto> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaDoctores); 

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Email", email);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<DoctoresDto>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<DoctoresDto>());

        }
    }
}