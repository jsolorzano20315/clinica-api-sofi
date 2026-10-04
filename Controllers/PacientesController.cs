using ClinicaAPI.Data;
using ClinicaAPI.DTOs;
using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Text;
namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase
    {
        private readonly AuthService _authService;
        public PacientesController(IConfiguration configuration, AuthService authService)
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }
        public string ConnectionStrings { get; private set; }

        [HttpPost]
        [Route("GuardarExpedienteCompleto")]
        public async Task<IActionResult> GuardarExpedienteCompleto([FromBody] ExpedienteClinicoRequest model)
        {
            if (model?.Paciente == null)
            {
                return BadRequest(new
                {
                    success = false,
                    mensaje = "Debe proporcionar los datos del paciente."
                });
            }

            var paciente = model.Paciente;

            if (string.IsNullOrWhiteSpace(paciente.Nombre) ||
                string.IsNullOrWhiteSpace(paciente.Apellido) ||
                string.IsNullOrWhiteSpace(paciente.Clinica))
            {
                return BadRequest(new
                {
                    success = false,
                    mensaje = "Nombre, apellido y clínica son obligatorios."
                });
            }

            var fecha = DateTime.Now;
            var userName = User.Identity?.Name ?? "Anonimo";

            using var connection = new System.Data.SqlClient.SqlConnection(
                Configuration.GetConnectionString("EntitiesContext"));

            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction(
                System.Data.IsolationLevel.Serializable);

            try
            {
                // =====================================================
                // 1. VALIDAR SI EL PACIENTE YA EXISTE
                // =====================================================

                const string sqlExistePaciente = @"
                        SELECT COUNT(1)
                        FROM Paciente
                        WHERE Nombre = @Nombre
                          AND Apellido = @Apellido
                          AND FechaNacimiento = @FechaNacimiento
                          AND Clinica = @Clinica;";

                var existePaciente = await connection.ExecuteScalarAsync<int>(
                    sqlExistePaciente,
                    new
                    {
                        paciente.Nombre,
                        paciente.Apellido,
                        paciente.FechaNacimiento,
                        paciente.Clinica
                    },
                    transaction);

                if (existePaciente > 0)
                {
                    transaction.Rollback();

                    return Conflict(new
                    {
                        success = false,
                        mensaje = "Ya existe un paciente con esos datos en esta clínica."
                    });
                }

                // =====================================================
                // 2. GUARDAR PACIENTE Y RECUPERAR ID
                // =====================================================

                var parametrosPaciente = new Dapper.DynamicParameters();

                parametrosPaciente.Add("@Nombre", paciente.Nombre);
                parametrosPaciente.Add("@Apellido", paciente.Apellido);
                parametrosPaciente.Add("@FechaNacimiento", paciente.FechaNacimiento);
                parametrosPaciente.Add("@Fecha", fecha);
                parametrosPaciente.Add("@Telefono", paciente.Telefono);
                parametrosPaciente.Add("@Genero", paciente.Genero);
                parametrosPaciente.Add("@EstadoCivil", paciente.EstadoCivil);
                parametrosPaciente.Add("@Direccion", paciente.Direccion);
                parametrosPaciente.Add("@Clinica", paciente.Clinica);

                int idPaciente = await connection.QuerySingleAsync<int>(
                    StaticResources.QueryCrearPacientes,
                    parametrosPaciente,
                    transaction);

                paciente.Id = idPaciente;

                // =====================================================
                // 3. FUNCIÓN PARA VALIDAR Y AGREGAR SECCIONES
                // =====================================================

                var seccionesGuardadas = new List<string>();

                async Task GuardarSeccion(
                    string tabla,
                    string nombreSeccion,
                    string query,
                    Dapper.DynamicParameters parametros)
                {
                    // Validar que no exista ya información para el paciente
                    string sqlExiste = $@"
                        SELECT COUNT(1)
                        FROM dbo.[{tabla}]
                        WHERE IdPaciente = @IdPaciente
                          AND Clinica = @Clinica;";

                    int existe = await connection.ExecuteScalarAsync<int>(
                        sqlExiste,
                        new
                        {
                            IdPaciente = idPaciente,
                            Clinica = paciente.Clinica
                        },
                        transaction);

                    if (existe > 0)
                    {
                        throw new InvalidOperationException(
                            $"Ya existen registros de {nombreSeccion} para este paciente.");
                    }

                    // Agregar parámetros comunes
                    parametros.Add("@IdPaciente", idPaciente);
                    parametros.Add("@Clinica", paciente.Clinica);
                    parametros.Add("@Fecha", fecha);

                    // Ejecutar INSERT
                    await connection.ExecuteAsync(
                        query,
                        parametros,
                        transaction);

                    seccionesGuardadas.Add(nombreSeccion);
                }

                // =====================================================
                // 4. ANTECEDENTES PERSONALES
                // =====================================================

                if (model.AntecedentesPersonales != null)
                {
                    var p = model.AntecedentesPersonales;

                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@AntecedentesPersona", p.AntecedentesPersona);

                    await GuardarSeccion(
                        "AntecedentesPersonales",
                        "Antecedentes personales",
                        StaticResources.QueryCrearAntecedentesPersonales,
                        dp);
                }

                // =====================================================
                // 5. ANTECEDENTES FAMILIARES
                // =====================================================

                if (model.AntecedentesFamiliares != null)
                {
                    var p = model.AntecedentesFamiliares;

                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@AntecedentesFamilia", p.AntecedentesFamilia);

                    await GuardarSeccion(
                        "AntecedentesFamiliares",
                        "Antecedentes familiares",
                        StaticResources.QueryCrearAntecedentesFamiliares,
                        dp);
                }

                // =====================================================
                // 6. ANTECEDENTES QUIRÚRGICOS
                // =====================================================

                if (model.AntecedentesQuirurgicos != null)
                {
                    var p = model.AntecedentesQuirurgicos;

                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@AntecedentesQuirurgico", p.AntecedentesQuirurgico);

                    await GuardarSeccion(
                        "AntecedentesQuirurgicos",
                        "Antecedentes quirúrgicos",
                        StaticResources.QueryCrearAntecedentesQuirurgicos,
                        dp);
                }

                // =====================================================
                // 7. GINECO-OBSTÉTRICOS (SOLO GÉNERO FEMENINO)
                // =====================================================

                if (model.GinecoObstetricos != null &&
                    (paciente.Genero == "F" ||
                     paciente.Genero == "FEMENINO"))
                {
                    var p = model.GinecoObstetricos;

                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@Gestaciones", p.Gestaciones);
                    dp.Add("@Partos", p.Partos);
                    dp.Add("@Cesareas", p.Cesareas);
                    dp.Add("@Abortos", p.Abortos);
                    dp.Add("@HijosVivos", p.HijosVivos);
                    dp.Add("@HijosMuertos", p.HijosMuertos);

                    await GuardarSeccion(
                        "GinecoObstetricos",
                        "Gineco-obstétricos",
                        StaticResources.QueryCrearGinecoObstetricos,
                        dp);
                }

                // =====================================================
                // 8. HÁBITOS
                // =====================================================

                if (model.Habitos != null)
                {
                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@DescripcionHabitos", model.Habitos.DescripcionHabitos);

                    await GuardarSeccion(
                        "Habitos", "Hábitos",
                        StaticResources.QueryCrearHabitos, dp);
                }

                // =====================================================
                // 9. INMUNIZACIÓN
                // =====================================================

                if (model.Inmunizacion != null)
                {
                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@EstadoInmunizacion", model.Inmunizacion.EstadoInmunizacion);

                    await GuardarSeccion(
                        "Inmunizacion", "Inmunización",
                        StaticResources.QueryCrearInmunizacion, dp);
                }

                // =====================================================
                // 10. ACTIVIDAD FÍSICA
                // =====================================================

                if (model.ActividadFisica != null)
                {
                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@NivelActividadFisica", model.ActividadFisica.NivelActividadFisica);

                    await GuardarSeccion(
                        "ActividadFisica", "Actividad física",
                        StaticResources.QueryCrearActividadFisica, dp);
                }

                // =====================================================
                // 11. ALERGIAS
                // =====================================================

                if (model.Alergias != null)
                {
                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@EstadoAlergia", model.Alergias.EstadoAlergia);
                    dp.Add("@Alergia", model.Alergias.Alergia);

                    await GuardarSeccion(
                        "Alergias", "Alergias",
                        StaticResources.QueryCrearAlergias, dp);
                }

                // =====================================================
                // 12. MEDICACIÓN ACTUAL
                // =====================================================

                if (model.MedicacionActual != null)
                {
                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@Medicacion", model.MedicacionActual.Medicacion);

                    await GuardarSeccion(
                        "MedicacionActual", "Medicación actual",
                        StaticResources.QueryCrearMedicacionActual, dp);
                }

                // =====================================================
                // 13. HISTORIA DE ENFERMEDAD ACTUAL
                // =====================================================

                if (model.HEA != null)
                {
                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@HistoriaEnfermedad", model.HEA.HistoriaEnfermedad);

                    await GuardarSeccion(
                        "HistoriaEnfermedadActual", "Historia de enfermedad actual",
                        StaticResources.QueryCrearHEA, dp);
                }

                // =====================================================
                // 14. EXAMEN FÍSICO
                // =====================================================

                if (model.ExamenFisico != null)
                {
                    var p = model.ExamenFisico;

                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@PresionArterial", p.PresionArterial);
                    dp.Add("@FrecuenciaCardiaca", p.FrecuenciaCardiaca);
                    dp.Add("@FrecuenciaRespiratoria", p.FrecuenciaRespiratoria);
                    dp.Add("@SaturacionOxigeno", p.SaturacionOxigeno);
                    dp.Add("@Peso", p.Peso);
                    dp.Add("@Temperatura", p.Temperatura);

                    await GuardarSeccion(
                        "ExamenFisico", "Examen físico",
                        StaticResources.QueryCrearExamenFisico, dp);
                }

                // =====================================================
                // 15. MC / IMC
                // =====================================================

                if (model.MC != null)
                {
                    var p = model.MC;

                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@Peso", p.Peso);
                    dp.Add("@Estatura", p.Estatura);
                    dp.Add("@IndiceMasaCorporal", p.IndiceMasaCorporal);

                    await GuardarSeccion(
                        "MC", "MC",
                        StaticResources.QueryCrearMC, dp);
                }

                // =====================================================
                // 16. ROAS
                // =====================================================

                if (model.ROAS != null)
                {
                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@RevisionAparatosSistemas", model.ROAS.RevisionAparatosSistemas);

                    await GuardarSeccion(
                        "ROAS", "ROAS",
                        StaticResources.QueryCrearROAS, dp);
                }

                // =====================================================
                // 17. LABORATORIOS
                // =====================================================

                if (model.Laboratorios != null)
                {
                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@ResultadosLaboratorio", model.Laboratorios.ResultadosLaboratorio);

                    await GuardarSeccion(
                        "Laboratorios", "Laboratorios",
                        StaticResources.QueryCrearLaboratorios, dp);
                }

                // =====================================================
                // 18. ECG
                // =====================================================

                if (model.ECG != null)
                {
                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@InterpretacionElectrocardiograma",
                        model.ECG.InterpretacionElectrocardiograma);

                    await GuardarSeccion(
                        "ECG", "ECG",
                        StaticResources.QueryCrearECG, dp);
                }

                // =====================================================
                // 19. IMÁGENES
                // =====================================================

                if (model.Imagenes != null)
                {
                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@EstudiosImagen", model.Imagenes.EstudiosImagen);

                    await GuardarSeccion(
                        "Imagenes", "Imágenes",
                        StaticResources.QueryCrearImagenes, dp);
                }

                // =====================================================
                // 20. RIESGO CARDIOVASCULAR
                // =====================================================

                if (model.RiesgoCardiovascular != null)
                {
                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@ResultadoEvaluacion",
                        model.RiesgoCardiovascular.ResultadoEvaluacion);

                    await GuardarSeccion(
                        "RiesgoCardiovascular", "Riesgo cardiovascular",
                        StaticResources.QueryCrearRiesgoCardiovascular, dp);
                }

                // =====================================================
                // 21. IMPRESIÓN DIAGNÓSTICA
                // =====================================================

                if (model.ImpresionDiagnostica != null)
                {
                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@Diagnostica", model.ImpresionDiagnostica.Diagnostica);

                    await GuardarSeccion(
                        "ImpresionDiagnostica", "Impresión diagnóstica",
                        StaticResources.QueryCrearImpresionDiagnostica, dp);
                }

                // =====================================================
                // 22. PLAN TERAPÉUTICO
                // =====================================================

                if (model.PlanTerapeutico != null)
                {
                    var dp = new Dapper.DynamicParameters();
                    dp.Add("@TratamientoIndicado",
                        model.PlanTerapeutico.TratamientoIndicado);

                    await GuardarSeccion(
                        "PlanTerapeutico", "Plan terapéutico",
                        StaticResources.QueryCrearPlanTerapeutico, dp);
                }

                // =====================================================
                // 23. CONFIRMAR TRANSACCIÓN
                // =====================================================

                transaction.Commit();

                return Ok(new
                {
                    success = true,
                    mensaje = "Expediente clínico guardado correctamente.",
                    idPaciente = idPaciente,
                    paciente = paciente,
                    seccionesGuardadas = seccionesGuardadas
                });
            }
            catch (InvalidOperationException ex)
            {
                transaction.Rollback();

                return Conflict(new
                {
                    success = false,
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                return StatusCode(500, new
                {
                    success = false,
                    mensaje = "Error al guardar el expediente clínico. Se revirtieron los cambios.",
                    error = ex.Message
                });
            }
        }

        [HttpPost()]
        [Route("GuardarPacientes")]
        public async Task<IActionResult> GuardarPacientes(Paciente model)
        {
            try
            {
                var userName = User.Identity?.Name ?? "Anonimo";

                var query = new StringBuilder();

                query.AppendLine(StaticResources.QueryCrearPacientes);

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@Nombre", model.Nombre);
                parameters.Add("@Apellido", model.Apellido);
                parameters.Add("@FechaNacimiento", model.FechaNacimiento);
                parameters.Add("@Fecha", DateTime.Now);
                parameters.Add("@Telefono", model.Telefono);
                parameters.Add("@Genero", model.Genero);
                parameters.Add("@EstadoCivil", model.EstadoCivil);
                parameters.Add("@Direccion", model.Direccion);
                parameters.Add("@Clinica", model.Clinica);

                using var connection = new System.Data.SqlClient.SqlConnection(
                    Configuration.GetConnectionString("EntitiesContext")
                );

                await connection.OpenAsync();

                // Recuperar el ID generado por SQL Server
                int idPaciente = await connection.QuerySingleAsync<int>(
                    query.ToString(),
                    parameters
                );

                // Asignar el ID al modelo
                model.Id = idPaciente;

                // Devolver el paciente registrado con su ID
                return Ok(new
                {
                    success = true,
                    mensaje = "Paciente guardado correctamente",
                    idPaciente = idPaciente,
                    paciente = model
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    mensaje = "Error al guardar el paciente",
                    error = ex.Message
                });
            }
        }

        [HttpPut()]
        [Route("EditarPacientes/{id}")]
        public async Task<IActionResult> EditarPacientes(int id, Paciente model)
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
                    FROM Paciente
                    WHERE Id = @Id;
                ";

                var existe = await connection.ExecuteScalarAsync<int>(
                    sqlExiste,
                    new
                    {
                        Id = id
                    }
                );

                // =========================================================
                // 2. SI NO EXISTE -> INSERTAR
                // =========================================================

                if (existe == 0)
                {
                    var queryGuardar = new StringBuilder();

                    queryGuardar.AppendLine(
                        StaticResources.QueryCrearPacientes
                    );

                    DynamicParameters parametrosGuardar =
                        new DynamicParameters();

                    parametrosGuardar.Add(
                        "@Nombre",
                        model.Nombre
                    );

                    parametrosGuardar.Add(
                        "@Apellido",
                        model.Apellido
                    );

                    parametrosGuardar.Add(
                        "@FechaNacimiento",
                        model.FechaNacimiento
                    );

                    parametrosGuardar.Add(
                        "@Telefono",
                        model.Telefono
                    );

                    parametrosGuardar.Add(
                        "@Genero",
                        model.Genero
                    );

                    parametrosGuardar.Add(
                        "@EstadoCivil",
                        model.EstadoCivil
                    );

                    parametrosGuardar.Add(
                        "@Direccion",
                        model.Direccion
                    );

                    var resultadoGuardar =
                        (await connection.QueryAsync<Paciente>(
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
                    StaticResources.QueryModificarPacientes
                );

                DynamicParameters parametrosEditar =
                    new DynamicParameters();

                parametrosEditar.Add(
                    "@Id",
                    id
                );

                parametrosEditar.Add(
                    "@Nombre",
                    model.Nombre
                );

                parametrosEditar.Add(
                    "@Apellido",
                    model.Apellido
                );

                parametrosEditar.Add(
                    "@FechaNacimiento",
                    model.FechaNacimiento
                );

                parametrosEditar.Add(
                    "@Telefono",
                    model.Telefono
                );

                parametrosEditar.Add(
                    "@Genero",
                    model.Genero
                );

                parametrosEditar.Add(
                    "@EstadoCivil",
                    model.EstadoCivil
                );

                parametrosEditar.Add(
                    "@Direccion",
                    model.Direccion
                );

                var resultadoEditar =
                    (await connection.QueryAsync<Paciente>(
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
        //[Route("EditarPacientes/{id}")]
        //public async Task<IActionResult> EditarPacientes(Paciente model) 
        //{

        //    var userName = User.Identity?.Name ?? "Anonimo";

        //    List<Paciente> result;
        //    var query = new StringBuilder();
        //    query.AppendLine(StaticResources.QueryModificarPacientes);

        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@Id", model.Id);
        //    parameters.Add("@Nombre", model.Nombre);
        //    parameters.Add("@Apellido", model.Apellido);
        //    parameters.Add("@FechaNacimiento", model.FechaNacimiento);
        //    parameters.Add("@Telefono", model.Telefono);
        //    parameters.Add("@Genero", model.Genero);
        //    parameters.Add("@EstadoCivil", model.EstadoCivil);
        //    parameters.Add("@Direccion", model.Direccion); 

        //    using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
        //    result = (await connection.QueryAsync<Paciente>(query.ToString(), parameters)).ToList();

        //    return Ok(result ?? new List<Paciente>());
        //}

        [HttpDelete()]
        [Route("EliminarPacientes/{id}")]
        public async Task<IActionResult> EliminarPacientes(int id)
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<Paciente> result;
            var query = new StringBuilder();
            query.AppendLine(StaticResources.QueryEliminarPacientes);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", id);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<Paciente>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<Paciente>());
        }

        [HttpGet()]
        [Route("ListaPacientes/{clinica}")]
        public async Task<IActionResult> ListaPacientes(string clinica)
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            List<PacienteDto> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaPacientes);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Clinica", clinica);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<PacienteDto>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<PacienteDto>());

        }

        [HttpGet()]
        [Route("ListaReportePacientes/{clinica}/{fechaInicio}/{fechaFin}")]
        public async Task<IActionResult> ListaReportePacientes(string clinica, DateTime fechaInicio, DateTime fechaFin)
        {

            var userName = User.Identity?.Name ?? "Anonimo";

            fechaInicio = fechaInicio.Date;
            fechaFin = fechaFin.Date.AddDays(1).AddTicks(-1);

            List<PacienteDto> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryListaPacientesFecha);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Clinica", clinica);
            parameters.Add("@FechaInicio", fechaInicio);
            parameters.Add("@FechaFin", fechaFin);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<PacienteDto>(query.ToString(), parameters)).ToList();

            return Ok(result ?? new List<PacienteDto>());

        }

        [HttpGet()]
        [Route("TotalPacientes/{clinica}")]
        public async Task<IActionResult> TotalPacientes(string clinica)
        {

            var query = new StringBuilder();
            query.AppendLine(StaticResources.QueryTotalPacientes);

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Clinica", clinica);

            using var connection = new SqlConnection(Configuration.GetConnectionString("EntitiesContext"));

            var result = await connection.QueryFirstOrDefaultAsync<int>(
                query.ToString(),
                parameters
            );

            return Ok(result);

        }   

        // ============================================================
        // SUBIR DOCUMENTOS / IMÁGENES DEL PACIENTE
        // ============================================================

        [HttpPost]
        [Route("SubirImagenes")]
        [RequestSizeLimit(100 * 1024 * 1024)]
        public async Task<IActionResult> SubirImagenes([FromForm] int IdPaciente, [FromForm] string Clinica, [FromForm] List<IFormFile> Archivos)
        {
            // =========================================================
            // CONEXIÓN SQL
            // =========================================================

            using var connection = new SqlConnection(
                Configuration.GetConnectionString("EntitiesContext")
            );

            await connection.OpenAsync();

            // =========================================================
            // VALIDAR PACIENTE
            // =========================================================

            if (IdPaciente <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    mensaje = "El IdPaciente es obligatorio."
                });
            }

            // =========================================================
            // VALIDAR CLÍNICA
            // =========================================================

            if (string.IsNullOrWhiteSpace(Clinica))
            {
                return BadRequest(new
                {
                    success = false,
                    mensaje = "La clínica es obligatoria."
                });
            }

            // =========================================================
            // VALIDAR ARCHIVOS
            // =========================================================

            if (Archivos == null || Archivos.Count == 0)
            {
                return BadRequest(new
                {
                    success = false,
                    mensaje = "Debe seleccionar al menos un archivo."
                });
            }

            // =========================================================
            // VALIDAR QUE EL PACIENTE EXISTA
            // =========================================================

            const string sqlPaciente = @"
                        SELECT COUNT(1)
                        FROM Paciente
                        WHERE Id = @IdPaciente
                          AND Clinica = @Clinica;
                    ";

            var existePaciente = await connection.ExecuteScalarAsync<int>(
                sqlPaciente,
                new
                {
                    IdPaciente,
                    Clinica
                }
            );

            if (existePaciente == 0)
            {
                return NotFound(new
                {
                    success = false,
                    mensaje = "El paciente no existe en la clínica indicada."
                });
            }

            // =========================================================
            // OBTENER CONFIGURACIÓN
            // =========================================================

            var rutaBase = Configuration[
                "ArchivosPacientes:RutaBase"
            ];

            if (string.IsNullOrWhiteSpace(rutaBase))
            {
                return StatusCode(500, new
                {
                    success = false,
                    mensaje =
                        "No está configurada la ruta de almacenamiento de documentos."
                });
            }

            var tamanoMaximoMB =
                Configuration.GetValue<int?>(
                    "ArchivosPacientes:TamanoMaximoMB"
                ) ?? 10;

            long tamanoMaximoBytes =
                tamanoMaximoMB * 1024L * 1024L;

            // =========================================================
            // EXTENSIONES PERMITIDAS
            // =========================================================

            var extensionesPermitidas =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase)
                {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp",
                ".pdf"
                };

            // =========================================================
            // CREAR CARPETA
            // =========================================================

            var clinicaSegura =
                LimpiarNombreCarpeta(Clinica);

            var carpetaPaciente = Path.Combine(
                rutaBase,
                clinicaSegura,
                $"Paciente_{IdPaciente}"
            );

            Directory.CreateDirectory(carpetaPaciente);

            // =========================================================
            // LISTA DE ARCHIVOS GUARDADOS
            // =========================================================

            var archivosGuardados = new List<object>();

            // =========================================================
            // PROCESAR ARCHIVOS
            // =========================================================

            foreach (var archivo in Archivos)
            {
                // -----------------------------------------------------
                // VALIDAR ARCHIVO
                // -----------------------------------------------------

                if (archivo == null || archivo.Length == 0)
                {
                    continue;
                }

                // -----------------------------------------------------
                // VALIDAR TAMAÑO
                // -----------------------------------------------------

                if (archivo.Length > tamanoMaximoBytes)
                {
                    return BadRequest(new
                    {
                        success = false,
                        mensaje =
                            $"El archivo '{archivo.FileName}' supera el tamaño máximo permitido de {tamanoMaximoMB} MB."
                    });
                }

                // -----------------------------------------------------
                // EXTENSIÓN
                // -----------------------------------------------------

                var extension =
                    Path.GetExtension(archivo.FileName);

                if (string.IsNullOrWhiteSpace(extension) ||
                    !extensionesPermitidas.Contains(extension))
                {
                    return BadRequest(new
                    {
                        success = false,
                        mensaje =
                            $"El archivo '{archivo.FileName}' no tiene una extensión permitida."
                    });
                }

                // -----------------------------------------------------
                // NOMBRE ORIGINAL
                // -----------------------------------------------------

                var nombreOriginal =
                    Path.GetFileName(archivo.FileName);

                var nombreSinExtension =
                    Path.GetFileNameWithoutExtension(
                        nombreOriginal
                    );

                nombreSinExtension =
                    LimpiarNombreArchivo(
                        nombreSinExtension
                    );

                // -----------------------------------------------------
                // GENERAR NOMBRE ÚNICO
                // -----------------------------------------------------

                var fechaArchivo =
                    DateTime.Now.ToString(
                        "yyyyMMdd_HHmmss"
                    );

                var identificador =
                    Guid.NewGuid()
                        .ToString("N")
                        .Substring(0, 8);

                var nombreArchivo =
                    $"{fechaArchivo}_{identificador}_{nombreSinExtension}{extension}";

                // -----------------------------------------------------
                // RUTA FÍSICA
                // -----------------------------------------------------

                var rutaArchivo =
                    Path.Combine(
                        carpetaPaciente,
                        nombreArchivo
                    );

                // -----------------------------------------------------
                // GUARDAR ARCHIVO FÍSICAMENTE
                // -----------------------------------------------------

                await using (
                    var stream = new FileStream(
                        rutaArchivo,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None
                    ))
                {
                    await archivo.CopyToAsync(stream);
                }

                // =====================================================
                // REGISTRAR EN dbo.DocumentosPaciente
                // =====================================================

                const string sqlDocumento = @"
                            INSERT INTO dbo.DocumentosPaciente
                            (
                                IdImagen,
                                NombreArchivo,
                                TipoArchivo,
                                RutaArchivo,
                                Fecha
                            )
                            VALUES
                            (
                                @IdImagen,
                                @NombreArchivo,
                                @TipoArchivo,
                                @RutaArchivo,
                                GETDATE()
                            );

                            SELECT CAST(SCOPE_IDENTITY() AS INT);
                        ";

                int idArchivoImagen =
                    await connection.QuerySingleAsync<int>(
                        sqlDocumento,
                        new
                        {
                            IdImagen = IdPaciente,
                            NombreArchivo = nombreArchivo,
                            TipoArchivo = archivo.ContentType,
                            RutaArchivo = rutaArchivo
                        }
                    );

                // =====================================================
                // AGREGAR A RESPUESTA
                // =====================================================

                archivosGuardados.Add(new
                {
                    idArchivoImagen = idArchivoImagen,
                    idImagen = IdPaciente,
                    nombreOriginal = nombreOriginal,
                    nombreArchivo = nombreArchivo,
                    tipoArchivo = archivo.ContentType,
                    rutaArchivo = rutaArchivo,
                    tamanoBytes = archivo.Length,
                    fecha = DateTime.Now
                });
            }

            // =========================================================
            // VALIDAR QUE HAYA ARCHIVOS
            // =========================================================

            if (archivosGuardados.Count == 0)
            {
                return BadRequest(new
                {
                    success = false,
                    mensaje = "No se pudo guardar ningún archivo."
                });
            }

            // =========================================================
            // RESPUESTA
            // =========================================================

            return Ok(new
            {
                success = true,
                mensaje =
                    "Los archivos fueron guardados y registrados correctamente.",
                idPaciente = IdPaciente,
                clinica = Clinica,
                cantidadArchivos = archivosGuardados.Count,
                archivos = archivosGuardados
            });
        }


         //  ============================================================
         //   LIMPIAR NOMBRE DE CARPETA
         //   ============================================================

            private static string LimpiarNombreCarpeta(string nombre)
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    return "SinClinica";

                foreach (var caracter in Path.GetInvalidFileNameChars())
                {
                    nombre = nombre.Replace(caracter, '_');
                }

                return nombre.Trim();
            }


         //   ============================================================
         //   LIMPIAR NOMBRE DE ARCHIVO
         //  ============================================================

            private static string LimpiarNombreArchivo(string nombre)
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    return "archivo";

                foreach (var caracter in Path.GetInvalidFileNameChars())
                {
                    nombre = nombre.Replace(caracter, '_');
                }

                return nombre.Trim();
            }


        // =========================================================
        // LISTAR DOCUMENTOS DE UN PACIENTE
        // =========================================================
        [HttpGet]
        [Route("ListaDocumentos/{idPaciente:int}")]
        public async Task<IActionResult> ListaDocumentos(int idPaciente)
        {
            try
            {
                if (idPaciente <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "El IdPaciente no es válido."
                    });
                }

                var connectionString =
                    Configuration.GetConnectionString("EntitiesContext");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "No se encontró la cadena de conexión 'EntitiesContext'."
                    });
                }

                using var connection = new SqlConnection(connectionString);

                const string sql = @"
            SELECT
                IdArchivoImagen,
                IdImagen,
                NombreArchivo,
                TipoArchivo,
                RutaArchivo,
                Fecha
            FROM dbo.DocumentosPaciente
            WHERE IdImagen = @IdPaciente
            ORDER BY Fecha DESC, IdArchivoImagen DESC;
        ";

                var documentos = await connection.QueryAsync(sql, new
                {
                    IdPaciente = idPaciente
                });

                return Ok(new
                {
                    success = true,
                    idPaciente = idPaciente,
                    cantidad = documentos.Count(),
                    documentos = documentos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al consultar los documentos del paciente.",
                    error = ex.Message
                });
            }
        }


        // =========================================================
        // DESCARGAR / VISUALIZAR DOCUMENTO
        // =========================================================
        [HttpGet]
        [Route("DescargarDocumento/{idDocumento:int}")]
        public async Task<IActionResult> DescargarDocumento(int idDocumento)
        {
            try
            {
                if (idDocumento <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "El IdDocumento no es válido."
                    });
                }

                var connectionString =
                    Configuration.GetConnectionString("EntitiesContext");

                using var connection = new SqlConnection(connectionString);

                const string sql = @"
                        SELECT
                            IdArchivoImagen,
                            NombreArchivo,
                            TipoArchivo,
                            RutaArchivo
                        FROM dbo.DocumentosPaciente
                        WHERE IdArchivoImagen = @IdDocumento;
                    ";

                var documento = await connection.QueryFirstOrDefaultAsync(sql, new
                {
                    IdDocumento = idDocumento
                });

                if (documento == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "El documento no existe."
                    });
                }

                string rutaArchivo = documento.RutaArchivo;

                if (string.IsNullOrWhiteSpace(rutaArchivo))
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "El documento no tiene una ruta registrada."
                    });
                }

                if (!System.IO.File.Exists(rutaArchivo))
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "El archivo físico no existe en el servidor."
                    });
                }

                var bytes = await System.IO.File.ReadAllBytesAsync(rutaArchivo);

                return File(
                    bytes,
                    documento.TipoArchivo,
                    documento.NombreArchivo
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al descargar el documento.",
                    error = ex.Message
                });
            }
        }


        // =========================================================
        // ELIMINAR DOCUMENTO
        // =========================================================
        [HttpDelete]
        [Route("EliminarDocumento/{idDocumento:int}")]
        public async Task<IActionResult> EliminarDocumento(int idDocumento)
        {
            string? rutaArchivo = null;

            try
            {
                if (idDocumento <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "El IdDocumento no es válido."
                    });
                }

                var connectionString =
                    Configuration.GetConnectionString("EntitiesContext");

                using var connection = new SqlConnection(connectionString);


                await connection.OpenAsync();

                using var transaction = connection.BeginTransaction();

                const string sqlBuscar = @"
                    SELECT
                        IdArchivoImagen,
                        NombreArchivo,
                        RutaArchivo
                    FROM dbo.DocumentosPaciente
                    WHERE IdArchivoImagen = @IdDocumento;
                ";

                var documento =
                    await connection.QueryFirstOrDefaultAsync(
                        sqlBuscar,
                        new
                        {
                            IdDocumento = idDocumento
                        },
                        transaction
                    );

                if (documento == null)
                {
                    transaction.Rollback();

                    return NotFound(new
                    {
                        success = false,
                        message = "El documento no existe."
                    });
                }

                rutaArchivo = documento.RutaArchivo;

                // -------------------------------------------------
                // ELIMINAR REGISTRO DE BASE DE DATOS
                // -------------------------------------------------

                const string sqlEliminar = @"
                        DELETE FROM dbo.DocumentosPaciente
                        WHERE IdArchivoImagen = @IdDocumento;
                    ";

                await connection.ExecuteAsync(
                    sqlEliminar,
                    new
                    {
                        IdDocumento = idDocumento
                    },
                    transaction
                );

                transaction.Commit();

                // -------------------------------------------------
                // ELIMINAR ARCHIVO FÍSICO
                // -------------------------------------------------

                if (!string.IsNullOrWhiteSpace(rutaArchivo) &&
                    System.IO.File.Exists(rutaArchivo))
                {
                    System.IO.File.Delete(rutaArchivo);
                }

                return Ok(new
                {
                    success = true,
                    message = "Documento eliminado correctamente.",
                    idDocumento = idDocumento
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al eliminar el documento.",
                    error = ex.Message
                });
            }
        }


        // =========================================================
        // VER DOCUMENTO
        // =========================================================

        [HttpGet]
        [Route("VerDocumento/{idDocumento:int}")]
        public async Task<IActionResult> VerDocumento(int idDocumento)
        {
            try
            {
                if (idDocumento <= 0)
                    return BadRequest(new
                    {
                        success = false,
                        message = "El IdDocumento no es válido."
                    });

                var connectionString =
                    Configuration.GetConnectionString("EntitiesContext");

                using var connection =
                    new SqlConnection(connectionString);

                const string sql = @"
                            SELECT
                                IdArchivoImagen,
                                NombreArchivo,
                                TipoArchivo,
                                RutaArchivo
                            FROM dbo.DocumentosPaciente
                            WHERE IdArchivoImagen = @IdDocumento;
                        ";

                var documento =
                    await connection.QueryFirstOrDefaultAsync(
                        sql,
                        new { IdDocumento = idDocumento }
                    );

                if (documento == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "El documento no existe."
                    });
                }

                string? rutaArchivo =
                    documento.RutaArchivo;

                if (string.IsNullOrWhiteSpace(rutaArchivo))
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "El documento no tiene una ruta registrada."
                    });
                }

                if (!System.IO.File.Exists(rutaArchivo))
                {
                    return NotFound(new
                    {
                        success = false,
                        message =
                            "El registro existe, pero el archivo físico no existe."
                    });
                }

                var bytes =
                    await System.IO.File.ReadAllBytesAsync(
                        rutaArchivo
                    );

                string tipoArchivo =
                    string.IsNullOrWhiteSpace(documento.TipoArchivo)
                        ? "application/octet-stream"
                        : documento.TipoArchivo;

                // IMPORTANTE:
                // No se envía el nombre del archivo como tercer parámetro.
                // Esto permite que el navegador intente visualizar
                // imágenes y PDF directamente.
                return File(
                    bytes,
                    tipoArchivo
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        success = false,
                        message = "Error al visualizar el documento.",
                        error = ex.Message
                    }
                );
            }
        }

    }

}
