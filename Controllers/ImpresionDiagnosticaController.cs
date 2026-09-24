using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImpresionDiagnosticaController : ControllerBase
    {
        private readonly AuthService _authService;
        public ImpresionDiagnosticaController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarImpresionDiagnostica")]
        public async Task<IActionResult> GuardarImpresionDiagnostica(ImpresionDiagnostica model)
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<ImpresionDiagnostica> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearImpresionDiagnostica);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdPaciente", model.IdPaciente);
            parameters.Add("@Clinica", model.Clinica);
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@Diagnostica", model.Diagnostica);


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<ImpresionDiagnostica>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<ImpresionDiagnostica>());
        }

        [HttpPut()]
        [Route("EditarImpresionDiagnostica/{id}")]
        public async Task<IActionResult> EditarImpresionDiagnostica(int id, ImpresionDiagnostica model)
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
                        FROM ImpresionDiagnostica
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
                        StaticResources.QueryCrearImpresionDiagnostica
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
                        "@Diagnostica",
                        model.Diagnostica
                    );

                    var resultadoGuardar =
                        (await connection.QueryAsync<ImpresionDiagnostica>(
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
                    StaticResources.QueryModificarImpresionDiagnostica
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
                    "@Diagnostica",
                    model.Diagnostica
                );

                var resultadoEditar =
                    (await connection.QueryAsync<ImpresionDiagnostica>(
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
        //[Route("EditarImpresionDiagnostica{id}")]
        //public async Task<IActionResult> EditarImpresionDiagnostica(ImpresionDiagnostica model) 
        //{

        //    var userName = User.Identity?.Name ?? "Anonimo";

        //    List<ImpresionDiagnostica> result;
        //    var query = new StringBuilder();
        //    query.AppendLine(StaticResources.QueryModificarImpresionDiagnostica);

        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@IdPaciente", model.IdPaciente);
        //    parameters.Add("@Fecha", DateTime.Now);
        //    parameters.Add("@Diagnostica", model.Diagnostica);



        //    using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
        //    result = (await connection.QueryAsync<ImpresionDiagnostica>(query.ToString(), parameters)).ToList();

        //    return Ok(result ?? new List<ImpresionDiagnostica>());
        //}


        [HttpGet()]
        [Route("ListaImpresionDiagnostica{clinica}")]
        public async Task<IActionResult> ListaImpresionDiagnostica(string clinica)
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<ImpresionDiagnostica> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaImpresionDiagnostica);

            DynamicParameters parameters = new DynamicParameters();


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<ImpresionDiagnostica>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<ImpresionDiagnostica>());

        }
    }
}
