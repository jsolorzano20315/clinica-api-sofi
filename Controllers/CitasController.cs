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
    public class CitasController : ControllerBase
    {
        private readonly AuthService _authService;
        public CitasController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarCitas")]
        public async Task<IActionResult> GuardarCitas(Cita model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Cita> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearCitas);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@PacienteId", model.PacienteId);
            parameters.Add("@DoctorId", model.DoctorId);
            parameters.Add("@Fecha", model.Fecha);
            parameters.Add("@Motivo", model.Motivo);
            parameters.Add("@Tipo", model.Tipo);
            parameters.Add("@Telefono", model.Telefono);
            parameters.Add("@Estado", model.Estado);
            parameters.Add("@Clinica", model.Clinica); 

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Cita>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Cita>());
        }

        [HttpPut()]
        [Route("EditarCitas/{id}")]
        public async Task<IActionResult> EditarCitas(Cita model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Cita> result;
            var query = new StringBuilder();
            query.AppendLine(StaticResources.QueryModificarCitas);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", model.Id);
            parameters.Add("@Estado", model.Estado);
            parameters.Add("@Tipo", model.Tipo);
            parameters.Add("@Motivo", model.Motivo);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Cita>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Cita>());
        }

        [HttpDelete()]
        [Route("EliminarCitas/{id}")]
        public async Task<IActionResult> EliminarCitas(int id)
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Cita> result;
            var query = new StringBuilder();
            query.AppendLine(StaticResources.QueryEliminarCitas);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", id);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Cita>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Cita>());
        }


        [HttpGet()]
        [Route("ListaCitas/{clinica}")]
        public async Task<IActionResult> ListaCitas(string clinica)
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<CitaDto> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaCitas);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Clinica", clinica);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<CitaDto>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<CitaDto>());

        }

        [HttpGet()]
        [Route("ListaReporteCitas/{clinica}/{fechaInicio}/{fechaFin}")]
        public async Task<IActionResult> ListaReporteCitas(string clinica, DateTime fechaInicio, DateTime fechaFin)  
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            fechaInicio = fechaInicio.Date;
            fechaFin = fechaFin.Date.AddDays(1).AddTicks(-1);

            List<TotalCitasDto> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaCitasFecha);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Clinica", clinica);
            parameters.Add("@FechaInicio", fechaInicio);
            parameters.Add("@FechaFin", fechaFin);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<TotalCitasDto>(query.ToString(), parameters)).ToList();

          return Ok(result ?? new List<TotalCitasDto>());

        } 

        [HttpGet]
        [Route("TotalCitasConfi/{clinica}")]
        public async Task<IActionResult> TotalCitasConf(string clinica)
        {
            var query = new StringBuilder();
            query.AppendLine(StaticResources.QueryTotalCitasConfirmado);

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Clinica", clinica);

            using var connection = new SqlConnection(Configuration.GetConnectionString("EntitiesContext"));

            var result = await connection.QueryFirstOrDefaultAsync<int>(
                query.ToString(),
                parameters
            );

            return Ok(result);
        }

        [HttpGet()]
        [Route("TotalCitasPend/{clinica}")]
        public async Task<IActionResult> TotalCitasPend(string clinica) 
        {

            var query = new StringBuilder();
            query.AppendLine(StaticResources.QueryTotalCitasPendiente);

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Clinica", clinica);

            using var connection = new SqlConnection(Configuration.GetConnectionString("EntitiesContext"));

            var result = await connection.QueryFirstOrDefaultAsync<int>(
                query.ToString(),
                parameters
            );

            return Ok(result);

        }

        [HttpGet()]
        [Route("TotalCitasCance/{clinica}")]
        public async Task<IActionResult> TotalCitasCance(string clinica) 
        {

            var query = new StringBuilder();
            query.AppendLine(StaticResources.QueryTotalCitasCanceladas);

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
