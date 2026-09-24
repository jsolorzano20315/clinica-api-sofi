using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AntecedentesFamiliaresController : ControllerBase
    {
        private readonly AuthService _authService;
        public AntecedentesFamiliaresController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarAntecedentesFamiliares")]
        public async Task<IActionResult> GuardarAntecedentesFamiliares(AntecedentesFamiliares model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<AntecedentesFamiliares> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearAntecedentesFamiliares);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdPaciente", model.IdPaciente);
            parameters.Add("@Clinica", model.Clinica); 
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@AntecedentesFamilia", model.AntecedentesFamilia);
           
            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<AntecedentesFamiliares>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<AntecedentesFamiliares>());
        }

        [HttpPut()]
        [Route("EditarAntecedentesFamiliares/{id}")]
        public async Task<IActionResult> EditarAntecedentesFamiliares(
    int id,
    AntecedentesFamiliares model)
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
            FROM AntecedentesFamiliares
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
                        StaticResources.QueryCrearAntecedentesFamiliares
                    );

                    DynamicParameters parametrosGuardar = new DynamicParameters();

                    parametrosGuardar.Add("@IdPaciente", id);
                    parametrosGuardar.Add("@Clinica", model.Clinica);
                    parametrosGuardar.Add("@Fecha", DateTime.Now);
                    parametrosGuardar.Add(
                        "@AntecedentesFamilia",
                        model.AntecedentesFamilia
                    );

                    var resultadoGuardar =
                        (await connection.QueryAsync<AntecedentesFamiliares>(
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
                    StaticResources.QueryModificarAntecedentesFamiliares
                );

                DynamicParameters parametrosEditar = new DynamicParameters();

                parametrosEditar.Add("@IdPaciente", id);
                parametrosEditar.Add("@Clinica", model.Clinica);
                parametrosEditar.Add("@Fecha", DateTime.Now);
                parametrosEditar.Add(
                    "@AntecedentesFamilia",
                    model.AntecedentesFamilia
                );

                var resultadoEditar =
                    (await connection.QueryAsync<AntecedentesFamiliares>(
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

        //[HttpPut()]
        //[Route("EditarAntecedentesFamiliares/{id}")]
        //public async Task<IActionResult> EditarAntecedentesFamiliares(AntecedentesFamiliares model) 
        //{

        //    var userName = User.Identity?.Name ?? "Anonimo";

        //    List<AntecedentesFamiliares> result;
        //    var query = new StringBuilder();
        //    query.AppendLine(StaticResources.QueryModificarAntecedentesPersonales);

        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@IdPaciente", model.IdPaciente);
        //    parameters.Add("@Fecha", DateTime.Now);
        //    parameters.Add("@AntecedentesFamilia", model.AntecedentesFamilia);

        //    using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
        //    result = (await connection.QueryAsync<AntecedentesFamiliares>(query.ToString(), parameters)).ToList();

        //    return Ok(result ?? new List<AntecedentesFamiliares>());
        //}


        [HttpGet()]
        [Route("ListaAntecedentesFamiliares/{clinica}")]
        public async Task<IActionResult> ListaAntecedentesFamiliares(string clinica) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<AntecedentesFamiliares> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaAntecedentesFamiliares); 

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Clinica", clinica);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<AntecedentesFamiliares>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<AntecedentesFamiliares>());

        }
    }
}

