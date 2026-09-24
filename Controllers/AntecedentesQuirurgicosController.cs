using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;


namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AntecedentesQuirurgicosController : ControllerBase
    {
        private readonly AuthService _authService;
        public AntecedentesQuirurgicosController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarAntecedentesQuirurgicos")]
        public async Task<IActionResult> GuardarAntecedentesQuirurgicos(AntecedentesQuirurgicos model)
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<AntecedentesQuirurgicos> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearAntecedentesQuirurgicos);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdPaciente", model.IdPaciente);
            parameters.Add("@Clinica", model.Clinica);
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@AntecedentesQuirurgico", model.AntecedentesQuirurgico);
          
            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<AntecedentesQuirurgicos>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<AntecedentesQuirurgicos>());
        }

        [HttpPut()]
        [Route("EditarAntecedentesQuirurgicos/{id}")]
        public async Task<IActionResult> EditarAntecedentesQuirurgicos(
    int id,
    AntecedentesQuirurgicos model)
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
            FROM AntecedentesQuirurgicos
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
                        StaticResources.QueryCrearAntecedentesQuirurgicos
                    );

                    DynamicParameters parametrosGuardar =
                        new DynamicParameters();

                    parametrosGuardar.Add("@IdPaciente", id);
                    parametrosGuardar.Add("@Clinica", model.Clinica);
                    parametrosGuardar.Add("@Fecha", DateTime.Now);

                    parametrosGuardar.Add(
                        "@AntecedentesQuirurgico",
                        model.AntecedentesQuirurgico
                    ); 

                    var resultadoGuardar =
                        (await connection.QueryAsync<AntecedentesQuirurgicos>(
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
                    StaticResources.QueryModificarAntecedentesQuirurgicos
                );

                DynamicParameters parametrosEditar =
                    new DynamicParameters();

                parametrosEditar.Add("@IdPaciente", id);
                parametrosEditar.Add("@Clinica", model.Clinica);
                parametrosEditar.Add("@Fecha", DateTime.Now);

                parametrosEditar.Add(
                    "@AntecedentesQuirurgico",
                    model.AntecedentesQuirurgico
                );

                var resultadoEditar =
                    (await connection.QueryAsync<AntecedentesQuirurgicos>(
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
        //[Route("EditarAntecedentesQuirurgicos/{id}")]
        //public async Task<IActionResult> EditarAntecedentesQuirurgicos(AntecedentesQuirurgicos model) 
        //{

        //    var userName = User.Identity?.Name ?? "Anonimo";

        //    List<AntecedentesQuirurgicos> result;
        //    var query = new StringBuilder();
        //    query.AppendLine(StaticResources.QueryModificarAntecedentesQuirurgicos);

        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@IdPaciente", model.IdPaciente);
        //    parameters.Add("@Fecha", DateTime.Now);
        //    parameters.Add("@AntecedentesQuirurgico", model.AntecedentesQuirurgico);

        //    using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
        //    result = (await connection.QueryAsync<AntecedentesQuirurgicos>(query.ToString(), parameters)).ToList();

        //    return Ok(result ?? new List<AntecedentesQuirurgicos>());
        //}


        [HttpGet()]
        [Route("ListaAntecedentesQuirurgicos{clinica}")]
        public async Task<IActionResult> ListaAntecedentesQuirurgicos(string clinica) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<AntecedentesQuirurgicos> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaAntecedentesQuirurgicos);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Clinica", clinica);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<AntecedentesQuirurgicos>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<AntecedentesQuirurgicos>());

        }
    }
}
