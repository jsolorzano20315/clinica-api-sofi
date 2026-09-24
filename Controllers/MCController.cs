using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MCController : ControllerBase
    {
        private readonly AuthService _authService;
        public MCController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarMC")]
        public async Task<IActionResult> GuardarMC(MC model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<MC> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearMC);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdPaciente", model.IdPaciente);
            parameters.Add("@Clinica", model.Clinica);
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@Peso", model.Peso);
            parameters.Add("@Estatura", model.Estatura);
            parameters.Add("@IndiceMasaCorporal", model.IndiceMasaCorporal);
         

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<MC>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<MC>());
        }

        [HttpPut()]
        [Route("EditarMC/{id}")]
        public async Task<IActionResult> EditarMC(int id, MC model)
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
                        FROM MC
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
                        StaticResources.QueryCrearMC
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
                        "@Peso",
                        model.Peso
                    );

                    parametrosGuardar.Add(
                        "@Estatura",
                        model.Estatura
                    );

                    parametrosGuardar.Add(
                        "@IndiceMasaCorporal",
                        model.IndiceMasaCorporal
                    );

                    var resultadoGuardar =
                        (await connection.QueryAsync<MC>(
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
                    StaticResources.QueryModificarIMC
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
                    "@Peso",
                    model.Peso
                );

                parametrosEditar.Add(
                    "@Estatura",
                    model.Estatura
                );

                parametrosEditar.Add(
                    "@IndiceMasaCorporal",
                    model.IndiceMasaCorporal
                );

                var resultadoEditar =
                    (await connection.QueryAsync<MC>(
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
        //[Route("EditarMC/{id}")]
        //public async Task<IActionResult> EditarMC(MC model) 
        //{

        //    var userName = User.Identity?.Name ?? "Anonimo";

        //    List<MC> result;
        //    var query = new StringBuilder();
        //    query.AppendLine(StaticResources.QueryModificarIMC); 

        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@IdPaciente", model.IdPaciente);
        //    parameters.Add("@Clinica", model.Clinica);
        //    parameters.Add("@Fecha", DateTime.Now);
        //    parameters.Add("@Peso", model.Peso);
        //    parameters.Add("@Estatura", model.Estatura);
        //    parameters.Add("@IndiceMasaCorporal", model.IndiceMasaCorporal);


        //    using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
        //    result = (await connection.QueryAsync<MC>(query.ToString(), parameters)).ToList();

        //    return Ok(result ?? new List<MC>()); 
        //}


        [HttpGet()]
        [Route("ListaMC{clinica}")]
        public async Task<IActionResult> ListaMC(string clinica) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<MC> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaIMC);

            DynamicParameters parameters = new DynamicParameters();


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<MC>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<MC>());

        }
    }
}
