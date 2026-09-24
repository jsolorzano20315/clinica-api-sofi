using ClinicaAPI.DTOs;
using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;


namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AntecedentesPersonalesController : ControllerBase
    {
        private readonly AuthService _authService;
        public AntecedentesPersonalesController(IConfiguration configuration, AuthService authService)  
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarAntecedentesPersonales")]
        public async Task<IActionResult> GuardarAntecedentesPersonales(AntecedentesPersonales model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<AntecedentesPersonales> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearAntecedentesPersonales);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdPaciente", model.IdPaciente);
            parameters.Add("@Clinica", model.Clinica);
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@AntecedentesPersona", model.AntecedentesPersona);
          
            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<AntecedentesPersonales>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<AntecedentesPersonales>());
        }


        [HttpPut()]
        [Route("EditarAntecedentesPersonales/{id}")]
        public async Task<IActionResult> EditarAntecedentesPersonales(int id, AntecedentesPersonales model)
        {
            try
            {
                var userName = User.Identity?.Name ?? "Anonimo";

                using var connection = new System.Data.SqlClient.SqlConnection(
                    Configuration.GetConnectionString("EntitiesContext")
                );

                // =========================================================
                // 1. VALIDAR SI YA EXISTE EL REGISTRO
                // =========================================================
                const string sqlExiste = @"
            SELECT COUNT(1)
            FROM AntecedentesPersonales
            WHERE IdPaciente = @IdPaciente
              AND Clinica = @Clinica;
        ";

                var existe = await connection.ExecuteScalarAsync<int>(
                    sqlExiste,
                    new
                    {
                        IdPaciente = id,
                        Clinica = model.Clinica
                    }
                );

                // =========================================================
                // 2. SI NO EXISTE -> INSERTAR
                // =========================================================
                if (existe == 0)
                {
                    var queryGuardar = new StringBuilder();

                    queryGuardar.AppendLine(
                        StaticResources.QueryCrearAntecedentesPersonales
                    );

                    DynamicParameters parametrosGuardar = new DynamicParameters();

                    parametrosGuardar.Add("@IdPaciente", id);
                    parametrosGuardar.Add("@Clinica", model.Clinica);
                    parametrosGuardar.Add("@Fecha", DateTime.Now);
                    parametrosGuardar.Add(
                        "@AntecedentesPersona",
                        model.AntecedentesPersona
                    );

                    var resultadoGuardar =
                        (await connection.QueryAsync<AntecedentesPersonales>(
                            queryGuardar.ToString(),
                            parametrosGuardar
                        )).ToList();

                    return Ok(resultadoGuardar);
                }

                // =========================================================
                // 3. SI EXISTE -> ACTUALIZAR
                // =========================================================
                var queryEditar = new StringBuilder();

                queryEditar.AppendLine(
                    StaticResources.QueryModificarAntecedentesPersonales
                );

                DynamicParameters parametrosEditar = new DynamicParameters();

                parametrosEditar.Add("@IdPaciente", id);
                parametrosEditar.Add("@Fecha", DateTime.Now);
                parametrosEditar.Add(
                    "@AntecedentesPersona",
                    model.AntecedentesPersona
                );

                var resultadoEditar =
                    (await connection.QueryAsync<AntecedentesPersonales>(
                        queryEditar.ToString(),
                        parametrosEditar
                    )).ToList();

                return Ok(resultadoEditar);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }


        [HttpGet()]
        [Route("ListaAntecedentesPersonales/{clinica}")]
        public async Task<IActionResult> ListaAntecedentesPersonales(string clinica) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<AntecedentesPersonales> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaAntecedentesPersonales); 

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Clinica", clinica);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<AntecedentesPersonales>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<AntecedentesPersonales>());

        }
    }
}
