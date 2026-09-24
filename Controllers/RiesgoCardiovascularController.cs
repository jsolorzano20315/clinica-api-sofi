using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RiesgoCardiovascularController : ControllerBase
    {

        private readonly AuthService _authService;
        public RiesgoCardiovascularController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarRiesgoCardiovascular")]
        public async Task<IActionResult> GuardarRiesgoCardiovascular(RiesgoCardiovascular model)
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<RiesgoCardiovascular> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearRiesgoCardiovascular);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdPaciente", model.IdPaciente);
            parameters.Add("@Clinica", model.Clinica);
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@ResultadoEvaluacion", model.ResultadoEvaluacion);


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<RiesgoCardiovascular>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<RiesgoCardiovascular>());
        }


        [HttpPut()]
        [Route("EditarRiesgoCardiovascular/{id}")]
        public async Task<IActionResult> EditarRiesgoCardiovascular(int id, RiesgoCardiovascular model)
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
                                    FROM RiesgoCardiovascular
                                    WHERE IdPaciente = @IdPaciente;
                                ";

                        var existe = await connection.ExecuteScalarAsync<int>(
                            sqlExiste,
                            new
                            {
                                IdPaciente = id
                            }
                        );

                        // =========================================================
                        // 2. SI NO EXISTE -> INSERTAR
                        // =========================================================

                        if (existe == 0)
                        {
                            var queryGuardar = new StringBuilder();

                            queryGuardar.AppendLine(
                                StaticResources.QueryCrearRiesgoCardiovascular
                            );

                            DynamicParameters parametrosGuardar =
                                new DynamicParameters();

                            parametrosGuardar.Add(
                                "@IdPaciente",
                                id
                            );

                            parametrosGuardar.Add(
                                "@Clinica",
                                model.Clinica
                            );

                            parametrosGuardar.Add(
                                "@Fecha",
                                DateTime.Now
                            );

                            parametrosGuardar.Add(
                                "@ResultadoEvaluacion",
                                model.ResultadoEvaluacion
                            );

                            var resultadoGuardar =
                                (await connection.QueryAsync<RiesgoCardiovascular>(
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
                            StaticResources.QueryModificarRiesgoCardiovascular
                        );

                        DynamicParameters parametrosEditar =
                            new DynamicParameters();

                        parametrosEditar.Add(
                            "@IdPaciente",
                            id
                        );

                        parametrosEditar.Add(
                            "@Clinica",
                            model.Clinica
                        );

                        parametrosEditar.Add(
                            "@Fecha",
                            DateTime.Now
                        );

                        parametrosEditar.Add(
                            "@ResultadoEvaluacion",
                            model.ResultadoEvaluacion
                        );

                        var resultadoEditar =
                            (await connection.QueryAsync<RiesgoCardiovascular>(
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
        //[Route("EditarRiesgoCardiovascular{id}")]
        //public async Task<IActionResult> EditarRiesgoCardiovascular(RiesgoCardiovascular model) 
        //{

        //    var userName = User.Identity?.Name ?? "Anonimo";

        //    List<RiesgoCardiovascular> result;
        //    var query = new StringBuilder();
        //    query.AppendLine(StaticResources.QueryModificarRiesgoCardiovascular);

        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@IdPaciente", model.IdPaciente);
        //    parameters.Add("@Clinica", model.Clinica);
        //    parameters.Add("@Fecha", DateTime.Now);
        //    parameters.Add("@ResultadoEvaluacion", model.ResultadoEvaluacion);



        //    using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
        //    result = (await connection.QueryAsync<RiesgoCardiovascular>(query.ToString(), parameters)).ToList();

        //    return Ok(result ?? new List<RiesgoCardiovascular>());
        //}


        [HttpGet()]
        [Route("ListaRiesgoCardiovascular{clinica}")]
        public async Task<IActionResult> ListaRiesgoCardiovascular(string clinica) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<RiesgoCardiovascular> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaRiesgoCardiovascular); 

            DynamicParameters parameters = new DynamicParameters();


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<RiesgoCardiovascular>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<RiesgoCardiovascular>());

        }
    }
}


