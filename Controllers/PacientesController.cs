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

        [HttpPost]
        [Route("GuardarExpedienteCompleto")]
        public async Task<IActionResult> GuardarExpedienteCompleto( [FromBody] ExpedienteClinicoRequest model)
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
    }
}
