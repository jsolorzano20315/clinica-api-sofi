using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InmunizacionController : ControllerBase
    {
        private readonly AuthService _authService;
        public InmunizacionController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarInmunizacion")]
        public async Task<IActionResult> GuardarInmunizacion(Inmunizacion model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Inmunizacion> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearInmunizacion);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdPaciente", model.IdPaciente);
            parameters.Add("@Clinica", model.Clinica);
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@EstadoInmunizacion", model.EstadoInmunizacion);


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Inmunizacion>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Inmunizacion>());
        }


        [HttpPut()]
        [Route("EditarInmunizacion/{id}")]
        public async Task<IActionResult> EditarInmunizacion(int id, Inmunizacion model)
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
                                FROM Inmunizacion
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
                                StaticResources.QueryCrearInmunizacion
                            );

                            DynamicParameters parametrosGuardar =
                                new DynamicParameters();

                            parametrosGuardar.Add("@IdPaciente", id);
                            parametrosGuardar.Add("@Clinica", model.Clinica);
                            parametrosGuardar.Add("@Fecha", DateTime.Now);

                            parametrosGuardar.Add(
                                "@EstadoInmunizacion",
                                model.EstadoInmunizacion
                            );

                            var resultadoGuardar =
                                (await connection.QueryAsync<Inmunizacion>(
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
                            StaticResources.QueryModificarInmunizacion
                        );

                        DynamicParameters parametrosEditar =
                            new DynamicParameters();

                        parametrosEditar.Add("@IdPaciente", id);
                        parametrosEditar.Add("@Clinica", model.Clinica);
                        parametrosEditar.Add("@Fecha", DateTime.Now);

                        parametrosEditar.Add(
                            "@EstadoInmunizacion",
                            model.EstadoInmunizacion
                        );

                        var resultadoEditar =
                            (await connection.QueryAsync<Inmunizacion>(
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
        //[Route("EditarInmunizacion/{id}")]
        //public async Task<IActionResult> EditarInmunizacion(Inmunizacion model) 
        //{

        //    var userName = User.Identity?.Name ?? "Anonimo";

        //    List<Inmunizacion> result;
        //    var query = new StringBuilder();
        //    query.AppendLine(StaticResources.QueryModificarInmunizacion);

        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@IdPaciente", model.IdPaciente);
        //    parameters.Add("@Fecha", DateTime.Now);
        //    parameters.Add("@EstadoInmunizacion", model.EstadoInmunizacion);


        //    using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
        //    result = (await connection.QueryAsync<Inmunizacion>(query.ToString(), parameters)).ToList();

        //    return Ok(result ?? new List<Inmunizacion>());
        //}


        [HttpGet()]
        [Route("ListaInmunizacion{clinica}")]
        public async Task<IActionResult> ListaInmunizacion(string clinica) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Inmunizacion> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaInmunizacion); 

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Clinica", clinica);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Inmunizacion>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Inmunizacion>());

        }
    }
}
