using ClinicaAPI.DTOs;
using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamenFisicoController : ControllerBase
    {
        private readonly AuthService _authService;
        public ExamenFisicoController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarExamenFisico")]
        public async Task<IActionResult> GuardarExamenFisico(ExamenFisico model)
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<ExamenFisico> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearExamenFisico);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdPaciente", model.IdPaciente);
            parameters.Add("@Clinica", model.Clinica);
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@PresionArterial", model.PresionArterial);
            parameters.Add("@FrecuenciaCardiaca", model.FrecuenciaCardiaca);
            parameters.Add("@FrecuenciaRespiratoria", model.FrecuenciaRespiratoria);
            parameters.Add("@SaturacionOxigeno", model.SaturacionOxigeno);
            parameters.Add("@Peso", model.Peso);
            parameters.Add("@Temperatura", model.Temperatura);


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<ExamenFisico>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<ExamenFisico>());
        }


        [HttpPut()]
        [Route("EditarExamenFisico/{id}")]
        public async Task<IActionResult> EditarExamenFisico( int id, ExamenFisico model) 
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
                                FROM ExamenFisico
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
                                StaticResources.QueryCrearExamenFisico
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
                                "@PresionArterial",
                                model.PresionArterial
                            );

                            parametrosGuardar.Add(
                                "@FrecuenciaCardiaca",
                                model.FrecuenciaCardiaca
                            );

                            parametrosGuardar.Add(
                                "@FrecuenciaRespiratoria",
                                model.FrecuenciaRespiratoria
                            );

                            parametrosGuardar.Add(
                                "@SaturacionOxigeno",
                                model.SaturacionOxigeno
                            );

                            parametrosGuardar.Add(
                                "@Peso",
                                model.Peso
                            );

                            parametrosGuardar.Add(
                                "@Temperatura",
                                model.Temperatura
                            );

                            var resultadoGuardar =
                                (await connection.QueryAsync<ExamenFisico>(
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
                            StaticResources.QueryModificarExamenFisico
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
                            "@PresionArterial",
                            model.PresionArterial
                        );

                        parametrosEditar.Add(
                            "@FrecuenciaCardiaca",
                            model.FrecuenciaCardiaca
                        );

                        parametrosEditar.Add(
                            "@FrecuenciaRespiratoria",
                            model.FrecuenciaRespiratoria
                        );

                        parametrosEditar.Add(
                            "@SaturacionOxigeno",
                            model.SaturacionOxigeno
                        );

                        parametrosEditar.Add(
                            "@Peso",
                            model.Peso
                        );

                        parametrosEditar.Add(
                            "@Temperatura",
                            model.Temperatura
                        );

                        var resultadoEditar =
                            (await connection.QueryAsync<ExamenFisico>(
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
        //[Route("EditarExamenFisico/{id}")]
        //public async Task<IActionResult> EditarExamenFisico(ExamenFisico model)
        //{

        //    var userName = User.Identity?.Name ?? "Anonimo";

        //    List<ExamenFisico> result;
        //    var query = new StringBuilder();
        //    query.AppendLine(StaticResources.QueryModificarExamenFisico);

        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@IdPaciente", model.IdPaciente);    
        //    parameters.Add("@Fecha", DateTime.Now);
        //    parameters.Add("@PresionArterial", model.PresionArterial);
        //    parameters.Add("@FrecuenciaCardiaca", model.FrecuenciaCardiaca);
        //    parameters.Add("@FrecuenciaRespiratoria", model.FrecuenciaRespiratoria);
        //    parameters.Add("@SaturacionOxigeno", model.SaturacionOxigeno);
        //    parameters.Add("@Peso", model.Peso);
        //    parameters.Add("@Temperatura", model.Temperatura);


        //    using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
        //    result = (await connection.QueryAsync<ExamenFisico>(query.ToString(), parameters)).ToList();

        //    return Ok(result ?? new List<ExamenFisico>());
        //}


        [HttpGet()]
        [Route("ListaExamenFisico{clinica}")]
        public async Task<IActionResult> ListaExamenFisico(string clinica) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<ExamenFisico> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaExamenFisico); 

            DynamicParameters parameters = new DynamicParameters();


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<ExamenFisico>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<ExamenFisico>());

        }
    }
}


