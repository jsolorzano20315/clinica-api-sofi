using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LaboratoriosController : ControllerBase
    {
        private readonly AuthService _authService;
        public LaboratoriosController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarLaboratorios")] 
        public async Task<IActionResult> GuardarLaboratorios(Laboratorios model)
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Laboratorios> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearLaboratorios);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdPaciente", model.IdPaciente);
            parameters.Add("@Clinica", model.Clinica);
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@ResultadosLaboratorio", model.ResultadosLaboratorio);


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Laboratorios>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Laboratorios>());
        }


        [HttpPut()]
        [Route("EditarLaboratorios/{id}")]
        public async Task<IActionResult> EditarLaboratorios(int id, Laboratorios model)
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
                        FROM Laboratorios
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
                        StaticResources.QueryCrearLaboratorios
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
                        "@ResultadosLaboratorio",
                        model.ResultadosLaboratorio
                    );

                    var resultadoGuardar =
                        (await connection.QueryAsync<Laboratorios>(
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
                    StaticResources.QueryModificarLaboratorios
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
                    "@ResultadosLaboratorio",
                    model.ResultadosLaboratorio
                );

                var resultadoEditar =
                    (await connection.QueryAsync<Laboratorios>(
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
        //[Route("EditarLaboratorios/{id}")]
        //public async Task<IActionResult> EditarLaboratorios(Laboratorios model)
        //{

        //    var userName = User.Identity?.Name ?? "Anonimo";

        //    List<Laboratorios> result;
        //    var query = new StringBuilder();
        //    query.AppendLine(StaticResources.QueryModificarLaboratorios);

        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@IdPaciente", model.IdPaciente);
        //    parameters.Add("@Fecha", DateTime.Now);
        //    parameters.Add("@ResultadosLaboratorio", model.ResultadosLaboratorio);


        //    using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
        //    result = (await connection.QueryAsync<Laboratorios>(query.ToString(), parameters)).ToList();

        //    return Ok(result ?? new List<Laboratorios>());
        //}


        [HttpGet()]
        [Route("ListaLaboratorios{clinica}")]
        public async Task<IActionResult> ListaLaboratorios(string clinica) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Laboratorios> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaLaboratorios);

            DynamicParameters parameters = new DynamicParameters();


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Laboratorios>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Laboratorios>());

        }
    }
}

