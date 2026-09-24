using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HabitosController : ControllerBase
    {
        private readonly AuthService _authService;
        public HabitosController(IConfiguration configuration, AuthService authService)
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }


        [HttpPost()]
        [Route("GuardarHabitos")]
        public async Task<IActionResult> GuardarHabitos(Habitos model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Habitos> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearHabitos);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdPaciente", model.IdPaciente);
            parameters.Add("@Clinica", model.Clinica);
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@DescripcionHabitos", model.DescripcionHabitos);
           

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Habitos>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Habitos>());
        }


        [HttpPut()]
        [Route("EditarHabitos/{id}")]
        public async Task<IActionResult> EditarHabitos(int id, Habitos model) 
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
                            FROM Habitos
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
                                StaticResources.QueryCrearHabitos
                            );

                            DynamicParameters parametrosGuardar =
                                new DynamicParameters();

                            parametrosGuardar.Add("@IdPaciente", id);
                            parametrosGuardar.Add("@Clinica", model.Clinica);
                            parametrosGuardar.Add("@Fecha", DateTime.Now);

                            parametrosGuardar.Add(
                                "@DescripcionHabitos",
                                model.DescripcionHabitos
                            );

                            var resultadoGuardar =
                                (await connection.QueryAsync<Habitos>(
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
                            StaticResources.QueryModificarhabitos
                        );

                        DynamicParameters parametrosEditar =
                            new DynamicParameters();

                        parametrosEditar.Add("@IdPaciente", id);
                        parametrosEditar.Add("@Clinica", model.Clinica);
                        parametrosEditar.Add("@Fecha", DateTime.Now);

                        parametrosEditar.Add(
                            "@DescripcionHabitos",
                            model.DescripcionHabitos
                        );

                        var resultadoEditar =
                            (await connection.QueryAsync<Habitos>(
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
        //[Route("EditarHabitos/{id}")]
        //public async Task<IActionResult> EditarAntecedentesHabitos(Habitos model) 
        //{

        //    var userName = User.Identity?.Name ?? "Anonimo";

        //    List<Habitos> result;
        //    var query = new StringBuilder();
        //    query.AppendLine(StaticResources.QueryModificarhabitos); 

        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@IdPaciente", model.IdPaciente);
        //    parameters.Add("@Fecha", DateTime.Now);
        //    parameters.Add("@DescripcionHabitos", model.DescripcionHabitos);


        //    using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
        //    result = (await connection.QueryAsync<Habitos>(query.ToString(), parameters)).ToList();

        //    return Ok(result ?? new List<Habitos>());
        //}


        [HttpGet()]
        [Route("ListaHabitos{clinica}")]
        public async Task<IActionResult> ListaHabitos(string clinica)
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Habitos> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListahabitos);  

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Clinica", clinica);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Habitos>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Habitos>());

        }
    }
}
