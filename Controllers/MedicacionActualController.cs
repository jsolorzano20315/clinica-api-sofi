using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicacionActualController : ControllerBase
    {
        private readonly AuthService _authService;
        public MedicacionActualController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarMedicacionActual")]
        public async Task<IActionResult> GuardarMedicacionActual(MedicacionActual model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<MedicacionActual> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearMedicacionActual);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdPaciente", model.IdPaciente);
            parameters.Add("@Clinica", model.Clinica);
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@Medicacion", model.Medicacion);


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<MedicacionActual>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<MedicacionActual>());
        }


        [HttpPut()]
        [Route("EditarMedicacionActual/{id}")]
        public async Task<IActionResult> EditarMedicacionActual(int id, MedicacionActual model)
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
                        FROM MedicacionActual
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
                        StaticResources.QueryCrearMedicacionActual
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
                        "@Medicacion",
                        model.Medicacion
                    );

                    var resultadoGuardar =
                        (await connection.QueryAsync<MedicacionActual>(
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
                    StaticResources.QueryModificarMedicacionActual
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
                    "@Medicacion",
                    model.Medicacion
                );

                var resultadoEditar =
                    (await connection.QueryAsync<MedicacionActual>(
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
        //[Route("EditarMedicacionActual/{id}")]
        //public async Task<IActionResult> EditarMedicacionActual(MedicacionActual model)
        //{

        //    var userName = User.Identity?.Name ?? "Anonimo";

        //    List<MedicacionActual> result;
        //    var query = new StringBuilder();
        //    query.AppendLine(StaticResources.QueryModificarMedicacionActual);

        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@IdPaciente", model.IdPaciente);
        //    parameters.Add("@Fecha", DateTime.Now);
        //    parameters.Add("@Medicacion", model.Medicacion);


        //    using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
        //    result = (await connection.QueryAsync<MedicacionActual>(query.ToString(), parameters)).ToList();

        //    return Ok(result ?? new List<MedicacionActual>());
        //}


        [HttpGet()]
        [Route("ListaMedicacionActual{clinica}")]
        public async Task<IActionResult> ListaMedicacionActual(string clinica) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<MedicacionActual> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaMedicacionActual); 

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Clinica", clinica);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<MedicacionActual>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<MedicacionActual>());

        }
    }
}

