using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlergiasController : ControllerBase
    {
        private readonly AuthService _authService;
        public AlergiasController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarAlergias")]
        public async Task<IActionResult> GuardarAlergias(Alergias model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Alergias> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearAlergias);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdPaciente", model.IdPaciente);
            parameters.Add("@Clinica", model.Clinica);
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@EstadoAlergia", model.EstadoAlergia);
            parameters.Add("@Alergia", model.Alergia);


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Alergias>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Alergias>());
        }

        [HttpPut()]
        [Route("EditarAlergias/{id}")]
        public async Task<IActionResult> EditarAlergias(int id, Alergias model)
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
                            FROM Alergias
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
                        StaticResources.QueryCrearAlergias
                    );

                    DynamicParameters parametrosGuardar =
                        new DynamicParameters();

                    parametrosGuardar.Add("@IdPaciente", id);
                    parametrosGuardar.Add("@Clinica", model.Clinica);
                    parametrosGuardar.Add("@Fecha", DateTime.Now);

                    parametrosGuardar.Add(
                        "@EstadoAlergia",
                        model.EstadoAlergia
                    );

                    parametrosGuardar.Add(
                        "@Alergia",
                        model.Alergia
                    );

                    var resultadoGuardar =
                        (await connection.QueryAsync<Alergias>(
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
                    StaticResources.QueryModificarAlergias
                );

                DynamicParameters parametrosEditar =
                    new DynamicParameters();

                parametrosEditar.Add("@IdPaciente", id);
                parametrosEditar.Add("@Clinica", model.Clinica);
                parametrosEditar.Add("@Fecha", DateTime.Now);

                parametrosEditar.Add(
                    "@EstadoAlergia",
                    model.EstadoAlergia
                );

                parametrosEditar.Add(
                    "@Alergia",
                    model.Alergia
                );

                var resultadoEditar =
                    (await connection.QueryAsync<Alergias>(
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
        //[Route("EditarAlergias/{id}")]
        //public async Task<IActionResult> EditarAlergias(Alergias model)
        //{

        //    var userName = User.Identity?.Name ?? "Anonimo";

        //    List<Alergias> result;
        //    var query = new StringBuilder();
        //    query.AppendLine(StaticResources.QueryModificarAlergias);

        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@IdPaciente", model.IdPaciente);
        //    parameters.Add("@Fecha", DateTime.Now);
        //    parameters.Add("@EstadoAlergia", model.EstadoAlergia);
        //    parameters.Add("@Alergia", model.Alergia);


        //    using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
        //    result = (await connection.QueryAsync<Alergias>(query.ToString(), parameters)).ToList();

        //    return Ok(result ?? new List<Alergias>());
        //}


        [HttpGet()]
        [Route("ListaAlergias{clinica}")]
        public async Task<IActionResult> ListaAlergias(string clinica) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Alergias> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaAlergias); 

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Clinica", clinica);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Alergias>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Alergias>());

        }
    }
}


