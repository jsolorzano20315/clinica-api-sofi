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
    public class GinecoObstetricosController : ControllerBase
    {
        private readonly AuthService _authService;
        public GinecoObstetricosController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarGinecoObstetricos")]
        public async Task<IActionResult> GuardarGinecoObstetricos(GinecoObstetricos model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<GinecoObstetricos> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearGinecoObstetricos);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdPaciente", model.IdPaciente);
            parameters.Add("@Clinica", model.Clinica);
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@Gestaciones", model.Gestaciones);
            parameters.Add("@Partos", model.Partos);
            parameters.Add("@Cesareas", model.Cesareas);
            parameters.Add("@Abortos", model.Abortos);
            parameters.Add("@HijosVivos", model.HijosVivos);
            parameters.Add("@HijosMuertos", model.HijosMuertos);


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<GinecoObstetricos>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<GinecoObstetricos>());
        }


        [HttpPut()]
        [Route("EditarGinecoObstetricos/{id}")]
        public async Task<IActionResult> EditarGinecoObstetricos( int id, GinecoObstetricosDto model)
                {
                    try
                    {
                        var userName = User.Identity?.Name ?? "Anonimo";

                        using var connection = new System.Data.SqlClient.SqlConnection(
                            Configuration.GetConnectionString("EntitiesContext")
                        );

                        // =========================================================
                        // 0. VALIDAR GENERO DEL PACIENTE
                        // Solo se permite guardar/editar GinecoObstetricos
                        // cuando el género es FEMENINO (F).
                        //
                        // El género ya viene en model.Genero desde Vue,
                        // por lo que NO consultamos la tabla Paciente/Pacientes.
                        // =========================================================

                        var genero = model.Genero?.Trim();

                        if (!string.Equals(
                                genero,
                                "F",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            return BadRequest(new
                            {
                                mensaje = "Los datos gineco-obstétricos solo aplican para pacientes de género Femenino."
                            });
                        }

                        // =========================================================
                        // 1. VALIDAR SI YA EXISTE EL REGISTRO
                        // =========================================================
                        const string sqlExiste = @"
                            SELECT COUNT(1)
                            FROM GinecoObstetricos
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
                                StaticResources.QueryCrearGinecoObstetricos
                            );

                            DynamicParameters parametrosGuardar =
                                new DynamicParameters();

                            parametrosGuardar.Add("@IdPaciente", id);
                            parametrosGuardar.Add("@Clinica", model.Clinica);
                            parametrosGuardar.Add("@Fecha", DateTime.Now);

                            parametrosGuardar.Add(
                                "@Gestaciones",
                                model.Gestaciones
                            );

                            parametrosGuardar.Add(
                                "@Partos",
                                model.Partos
                            );

                            parametrosGuardar.Add(
                                "@Cesareas",
                                model.Cesareas
                            );

                            parametrosGuardar.Add(
                                "@Abortos",
                                model.Abortos
                            );

                            parametrosGuardar.Add(
                                "@HijosVivos",
                                model.HijosVivos
                            );

                            parametrosGuardar.Add(
                                "@HijosMuertos",
                                model.HijosMuertos
                            );

                            var resultadoGuardar =
                                (await connection.QueryAsync<GinecoObstetricosDto>(
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
                            StaticResources.QueryModificarGinecoObstetricos
                        );

                        DynamicParameters parametrosEditar =
                            new DynamicParameters();

                        parametrosEditar.Add("@IdPaciente", id);
                        parametrosEditar.Add("@Clinica", model.Clinica);
                        parametrosEditar.Add("@Fecha", DateTime.Now);

                        parametrosEditar.Add(
                            "@Gestaciones",
                            model.Gestaciones
                        );

                        parametrosEditar.Add(
                            "@Partos",
                            model.Partos
                        );

                        parametrosEditar.Add(
                            "@Cesareas",
                            model.Cesareas
                        );

                        parametrosEditar.Add(
                            "@Abortos",
                            model.Abortos
                        );

                        parametrosEditar.Add(
                            "@HijosVivos",
                            model.HijosVivos
                        );

                        parametrosEditar.Add(
                            "@HijosMuertos",
                            model.HijosMuertos
                        );

                        var resultadoEditar =
                            (await connection.QueryAsync<GinecoObstetricosDto>(
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
        //[Route("EditarGinecoObstetricos/{id}")]
        //public async Task<IActionResult> EditarGinecoObstetricos(GinecoObstetricos model) 
        //{

        //    var userName = User.Identity?.Name ?? "Anonimo";

        //    List<GinecoObstetricos> result;
        //    var query = new StringBuilder();
        //    query.AppendLine(StaticResources.QueryModificarGinecoObstetricos);

        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@IdPaciente", model.IdPaciente);
        //    parameters.Add("@Fecha", DateTime.Now);
        //    parameters.Add("@Gestaciones", model.Gestaciones);
        //    parameters.Add("@Partos", model.Partos);
        //    parameters.Add("@Cesareas", model.Cesareas);
        //    parameters.Add("@Abortos", model.Abortos);
        //    parameters.Add("@HijosVivos", model.HijosVivos);
        //    parameters.Add("@HijosMuertos", model.HijosMuertos);


        //    using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
        //    result = (await connection.QueryAsync<GinecoObstetricos>(query.ToString(), parameters)).ToList();

        //    return Ok(result ?? new List<GinecoObstetricos>());
        //}


        [HttpGet()]
        [Route("ListaGinecoObstetricos{clinica}")]
        public async Task<IActionResult> ListaGinecoObstetricos(string clinica)
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<GinecoObstetricos> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaGinecoObstetricos); 

            DynamicParameters parameters = new DynamicParameters();


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<GinecoObstetricos>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<GinecoObstetricos>());

        }
    }
}



