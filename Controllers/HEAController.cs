using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HEAController : ControllerBase
    {
        private readonly AuthService _authService;
        public HEAController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarHEA")]
        public async Task<IActionResult> GuardarHEA(HistoriaEnfermedadActual model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<HistoriaEnfermedadActual> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearHEA);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdPaciente", model.IdPaciente);
            parameters.Add("@Clinica", model.Clinica);
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@HistoriaEnfermedad", model.HistoriaEnfermedad);


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<HistoriaEnfermedadActual>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<HistoriaEnfermedadActual>());
        }

        
        [HttpPut()]
        [Route("EditarHistoriaEnfermedadActual/{id}")]
        public async Task<IActionResult> EditarHistoriaEnfermedadActual(int id, HistoriaEnfermedadActual model)
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
                                FROM HistoriaEnfermedadActual
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
                                StaticResources.QueryCrearHEA
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
                                "@HistoriaEnfermedad",
                                model.HistoriaEnfermedad
                            );

                            var resultadoGuardar =
                                (await connection.QueryAsync<HistoriaEnfermedadActual>(
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
                            StaticResources.QueryModificarHEA
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
                            "@HistoriaEnfermedad",
                            model.HistoriaEnfermedad
                        );

                        var resultadoEditar =
                            (await connection.QueryAsync<HistoriaEnfermedadActual>(
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
        //[Route("EditarHistoriaEnfermedadActual/{id}")]
        //public async Task<IActionResult> EditarHistoriaEnfermedadActual(HistoriaEnfermedadActual model)
        //{

        //    var userName = User.Identity?.Name ?? "Anonimo";

        //    List<HistoriaEnfermedadActual> result;
        //    var query = new StringBuilder();
        //    query.AppendLine(StaticResources.QueryModificarHEA); 

        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@IdPaciente", model.IdPaciente);
        //    parameters.Add("@Fecha", DateTime.Now);
        //    parameters.Add("@HistoriaEnfermedad", model.HistoriaEnfermedad);


        //    using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
        //    result = (await connection.QueryAsync<HistoriaEnfermedadActual>(query.ToString(), parameters)).ToList();

        //    return Ok(result ?? new List<HistoriaEnfermedadActual>());
        //}


        [HttpGet()]
        [Route("ListaHistoriaEnfermedadActual{clinica}")]
        public async Task<IActionResult> ListaHistoriaEnfermedadActual(string clinica) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<HistoriaEnfermedadActual> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaHEA);

            DynamicParameters parameters = new DynamicParameters();
             

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<HistoriaEnfermedadActual>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<HistoriaEnfermedadActual>());

        }
    }
}

