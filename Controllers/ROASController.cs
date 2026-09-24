using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ROASController : ControllerBase
    {
        private readonly AuthService _authService;
        public ROASController(IConfiguration configuration, AuthService authService)
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarROAS")]
        public async Task<IActionResult> GuardarROAS(ROAS model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<ROAS> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearROAS);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdPaciente", model.IdPaciente);
            parameters.Add("@Clinica", model.Clinica);
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@RevisionAparatosSistemas", model.RevisionAparatosSistemas);


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<ROAS>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<ROAS>());
        }

        [HttpPut()]
        [Route("EditarROAS/{id}")]
        public async Task<IActionResult> EditarROAS(int id, ROAS model)
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
                                FROM ROAS
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
                                StaticResources.QueryCrearROAS
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
                                "@RevisionAparatosSistemas",
                                model.RevisionAparatosSistemas
                            );

                            var resultadoGuardar =
                                (await connection.QueryAsync<ROAS>(
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
                            StaticResources.QueryModificarROAS
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
                            "@RevisionAparatosSistemas",
                            model.RevisionAparatosSistemas
                        );

                        var resultadoEditar =
                            (await connection.QueryAsync<ROAS>(
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
        //[Route("EditarROAS/{id}")]
        //public async Task<IActionResult> EditarROAS(ROAS model) 
        //{

        //    var userName = User.Identity?.Name ?? "Anonimo";

        //    List<ROAS> result;
        //    var query = new StringBuilder();
        //    query.AppendLine(StaticResources.QueryModificarROAS); 

        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@IdPaciente", model.IdPaciente);
        //    parameters.Add("@Fecha", DateTime.Now);
        //    parameters.Add("@RevisionAparatosSistemas", model.RevisionAparatosSistemas);


        //    using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
        //    result = (await connection.QueryAsync<ROAS>(query.ToString(), parameters)).ToList();

        //    return Ok(result ?? new List<ROAS>());
        //}


        [HttpGet()]
        [Route("ListaROAS{clinica}")]
        public async Task<IActionResult> ListaROAS(string clinica) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<ROAS> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaROAS);

            DynamicParameters parameters = new DynamicParameters();


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<ROAS>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<ROAS>());

        }
    }
}

