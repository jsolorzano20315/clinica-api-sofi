using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagenesController : ControllerBase
    {
        private readonly AuthService _authService;
        public ImagenesController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }

        [HttpPost()]
        [Route("GuardarImagenes")]
        public async Task<IActionResult> GuardarImagenes(Imagenes model) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Imagenes> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryCrearImagenes);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@IdPaciente", model.IdPaciente);
            parameters.Add("@Clinica", model.Clinica);
            parameters.Add("@Fecha", DateTime.Now);
            parameters.Add("@EstudiosImagen", model.EstudiosImagen);


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Imagenes>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Imagenes>());
        }


        [HttpPut()]
        [Route("EditarImagenes/{id}")]
        public async Task<IActionResult> EditarImagenes(int id, Imagenes model)
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
                                FROM Imagenes
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
                                StaticResources.QueryCrearImagenes
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
                                "@EstudiosImagen",
                                model.EstudiosImagen
                            );

                            var resultadoGuardar =
                                (await connection.QueryAsync<Imagenes>(
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
                            StaticResources.QueryModificarImagenes
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
                            "@EstudiosImagen",
                            model.EstudiosImagen
                        );

                        var resultadoEditar =
                            (await connection.QueryAsync<Imagenes>(
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
        //[Route("EditarImagenes/{id}")]
        //public async Task<IActionResult> EditarImagenes(Imagenes model)
        //{

        //    var userName = User.Identity?.Name ?? "Anonimo";

        //    List<Imagenes> result;
        //    var query = new StringBuilder();
        //    query.AppendLine(StaticResources.QueryModificarImagenes);

        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@IdPaciente", model.IdPaciente);
        //    parameters.Add("@Clinica", model.Clinica);
        //    parameters.Add("@Fecha", DateTime.Now);
        //    parameters.Add("@EstudiosImagen", model.EstudiosImagen);



        //    using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
        //    result = (await connection.QueryAsync<Imagenes>(query.ToString(), parameters)).ToList();

        //    return Ok(result ?? new List<Imagenes>());
        //}


        [HttpGet()]
        [Route("ListaImagenes{clinica}")]
        public async Task<IActionResult> ListaImagenes(string clinica) 
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Imagenes> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaImagenes);

            DynamicParameters parameters = new DynamicParameters();


            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Imagenes>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Imagenes>());

        }
    }
}
