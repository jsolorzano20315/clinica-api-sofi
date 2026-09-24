using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActividadFisicaController : ControllerBase
    {
        private readonly AuthService _authService;
        public ActividadFisicaController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarActividadFisica")]
        public async Task<IActionResult> GuardarActividadFisica(ActividadFisica model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<ActividadFisica> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearActividadFisica);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdPaciente", model.IdPaciente);
            parameters.Add("@Clinica", model.Clinica);
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@NivelActividadFisica", model.NivelActividadFisica);


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<ActividadFisica>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<ActividadFisica>());
        }



        [HttpPut()]
        [Route("EditarActividadFisica/{id}")]
        public async Task<IActionResult> EditarActividadFisica(int id, ActividadFisica model)
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
                                    FROM ActividadFisica
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
                                StaticResources.QueryCrearActividadFisica
                            );

                            DynamicParameters parametrosGuardar =
                                new DynamicParameters();

                            parametrosGuardar.Add("@IdPaciente", id);
                            parametrosGuardar.Add("@Clinica", model.Clinica);
                            parametrosGuardar.Add("@Fecha", DateTime.Now);

                            parametrosGuardar.Add(
                                "@NivelActividadFisica",
                                model.NivelActividadFisica
                            );

                            var resultadoGuardar =
                                (await connection.QueryAsync<ActividadFisica>(
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
                            StaticResources.QueryModificarActividadFisica
                        );

                        DynamicParameters parametrosEditar =
                            new DynamicParameters();

                        parametrosEditar.Add("@IdPaciente", id);
                        parametrosEditar.Add("@Clinica", model.Clinica);
                        parametrosEditar.Add("@Fecha", DateTime.Now);

                        parametrosEditar.Add(
                            "@NivelActividadFisica",
                            model.NivelActividadFisica
                        );

                        var resultadoEditar =
                            (await connection.QueryAsync<ActividadFisica>(
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
        //[Route("EditarActividadFisica/{id}")]
        //public async Task<IActionResult> EditarActividadFisica(ActividadFisica model) 
        //{

        //    var userName = User.Identity?.Name ?? "Anonimo";

        //    List<ActividadFisica> result;
        //    var query = new StringBuilder();
        //    query.AppendLine(StaticResources.QueryModificarActividadFisica); 

        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@IdPaciente", model.IdPaciente);
        //    parameters.Add("@Fecha", DateTime.Now);
        //    parameters.Add("@NivelActividadFisica", model.NivelActividadFisica);


        //    using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
        //    result = (await connection.QueryAsync<ActividadFisica>(query.ToString(), parameters)).ToList();

        //    return Ok(result ?? new List<ActividadFisica>());
        //}


        [HttpGet()]
        [Route("ListaActividadFisica{clinica}")]
        public async Task<IActionResult> ListaActividadFisica(string clinica) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<ActividadFisica> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaActividadFisica);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Clinica", clinica);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<ActividadFisica>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<ActividadFisica>());

        }
    }
}
