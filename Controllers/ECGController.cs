using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ECGController : ControllerBase
    {
        private readonly AuthService _authService;
        public ECGController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarECG")]
        public async Task<IActionResult> GuardarECG(ECG model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<ECG> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearECG);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdPaciente", model.IdPaciente);
            parameters.Add("@Clinica", model.Clinica);
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@InterpretacionElectrocardiograma", model.InterpretacionElectrocardiograma);


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<ECG>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<ECG>());
        }


        [HttpPut()]
        [Route("EditarECG/{id}")]
        public async Task<IActionResult> EditarECG(int id, ECG model)
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
                                FROM ECG
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
                                StaticResources.QueryCrearECG
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
                                "@InterpretacionElectrocardiograma",
                                model.InterpretacionElectrocardiograma
                            );

                            var resultadoGuardar =
                                (await connection.QueryAsync<ECG>(
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
                            StaticResources.QueryModificarECG
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
                            "@InterpretacionElectrocardiograma",
                            model.InterpretacionElectrocardiograma
                        );

                        var resultadoEditar =
                            (await connection.QueryAsync<ECG>(
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
        //[Route("EditarECG/{id}")]
        //public async Task<IActionResult> EditarECG(ECG model) 
        //{

        //    var userName = User.Identity?.Name ?? "Anonimo";

        //    List<ECG> result;
        //    var query = new StringBuilder();
        //    query.AppendLine(StaticResources.QueryModificarECG);

        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@IdPaciente", model.IdPaciente);
        //    parameters.Add("@Fecha", DateTime.Now);
        //    parameters.Add("@InterpretacionElectrocardiograma", model.InterpretacionElectrocardiograma);



        //    using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
        //    result = (await connection.QueryAsync<ECG>(query.ToString(), parameters)).ToList();

        //    return Ok(result ?? new List<ECG>());
        //}


        [HttpGet()]
        [Route("ListaECG{clinica}")]
        public async Task<IActionResult> ListaECG(string clinica)
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<ECG> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaECG);

            DynamicParameters parameters = new DynamicParameters();


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<ECG>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<ECG>());

        }
    }
}


