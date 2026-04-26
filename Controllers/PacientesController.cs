using ClinicaAPI.Data;
using ClinicaAPI.DTOs;
using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Text;
namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase 
    {
        private readonly AuthService _authService; 
        public PacientesController(IConfiguration configuration, AuthService authService)
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarPacientes")]
        public async Task<IActionResult> GuardarPacientes(Paciente model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Paciente> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearPacientes); 

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Nombre", model.Nombre);
            parameters.Add("@Apellido", model.Apellido);
            parameters.Add("@FechaNacimiento", model.FechaNacimiento);
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@Telefono", model.Telefono);
            parameters.Add("@Direccion", model.Direccion);
            parameters.Add("@Clinica", model.Clinica);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Paciente>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Paciente>());
        }

        [HttpPut()]
        [Route("EditarPacientes/{id}")]
        public async Task<IActionResult> EditarPacientes(Paciente model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Paciente> result;
            var query = new StringBuilder();
            query.AppendLine(StaticResources.QueryModificarPacientes);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", model.Id);
            parameters.Add("@Nombre", model.Nombre);
            parameters.Add("@Apellido", model.Apellido);
            parameters.Add("@FechaNacimiento", model.FechaNacimiento);
            parameters.Add("@Telefono", model.Telefono);
            parameters.Add("@Direccion", model.Direccion); 

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Paciente>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Paciente>());
        }

        [HttpDelete()]
        [Route("EliminarPacientes/{id}")]
        public async Task<IActionResult> EliminarPacientes(int id) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Paciente> result;
            var query = new StringBuilder();
            query.AppendLine(StaticResources.QueryEliminarPacientes);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", id);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Paciente>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Paciente>());
        }

        [HttpGet()]
        [Route("ListaPacientes/{clinica}")]
        public async Task<IActionResult> ListaPacientes(string clinica) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<PacienteDto> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaPacientes);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Clinica", clinica);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<PacienteDto>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<PacienteDto>());

        }

        [HttpGet()]
        [Route("ListaReportePacientes/{clinica}/{fechaInicio}/{fechaFin}")]
        public async Task<IActionResult> ListaReportePacientes(string clinica, DateTime fechaInicio, DateTime fechaFin) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            fechaInicio = fechaInicio.Date;
            fechaFin = fechaFin.Date.AddDays(1).AddTicks(-1);

            List<PacienteDto> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaPacientesFecha); 

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Clinica", clinica);
            parameters.Add("@FechaInicio", fechaInicio);
            parameters.Add("@FechaFin", fechaFin);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<PacienteDto>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<PacienteDto>());

        }

        [HttpGet()]
        [Route("TotalPacientes/{clinica}")]
        public async Task<IActionResult> TotalPacientes(string clinica) 
        {

            var query = new StringBuilder();
            query.AppendLine(StaticResources.QueryTotalPacientes);

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Clinica", clinica);

            using var connection = new SqlConnection(Configuration.GetConnectionString("EntitiesContext"));

            var result = await connection.QueryFirstOrDefaultAsync<int>(
                query.ToString(),
                parameters
            );

            return Ok(result);

        }
    }
}
