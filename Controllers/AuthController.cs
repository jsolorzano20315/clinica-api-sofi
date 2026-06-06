using ClinicaAPI.DTOs;
using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Identity;
using ClinicaAPI.Interface;
using System.Data.SqlClient;

namespace ClinicaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IEmailService _emailService;
        public AuthController(IConfiguration configuration, AuthService authService, IEmailService emailService)
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _emailService = emailService;
        }

        public IConfiguration Configuration { get; }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // VALIDACIONES
            if (
                request == null ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password)
            )
            {
                return BadRequest(new
                {
                    message = "Correo y contraseña son obligatorios"
                });
            }

            try
            {
                var query = new StringBuilder();

                // SOLO BUSCAR POR EMAIL
                query.AppendLine(@"
                        SELECT *
                        FROM Usuario
                        WHERE Email = @Email
                    ");

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add(
                    "@Email",
                    request.Email.Trim().ToLower()
                );

                using var connection =
                    new System.Data.SqlClient.SqlConnection(
                        Configuration.GetConnectionString("EntitiesContext")
                    );

                var result =
                    (await connection.QueryAsync<LoginResponse>(
                        query.ToString(),
                        parameters
                    )).ToList();

                // VALIDAR USUARIO
                var userDb = result.FirstOrDefault();

                if (userDb == null)
                {
                    return Unauthorized(new
                    {
                        message = "Correo o contraseña incorrectos"
                    });
                }

                // VALIDAR PASSWORD HASH
                bool passwordCorrecta =
                    BCrypt.Net.BCrypt.Verify(
                        request.Password,
                        userDb.Password
                    );

                if (!passwordCorrecta)
                {
                    return Unauthorized(new
                    {
                        message = "Correo o contraseña incorrectos"
                    });
                }

                // VALIDAR CORREO
                if (!userDb.CorreoVerificado)
                {
                    return Unauthorized(new
                    {
                        message = "Debe verificar su correo electrónico antes de iniciar sesión."
                    });
                }

                // CREAR USUARIO JWT
                var user = _authService.ValidateUser(
                    userDb.Nombre,
                    userDb.Rol,
                    userDb.Email,
                    userDb.Password,
                    userDb.Clinica
                );

                // TOKEN
                string token = _authService.GenerateToken(user);

                // RESPUESTA
                return Ok(new LoginResponse
                {
                    Nombre = user.Nombre,
                    Email = user.Email,
                    Rol = user.Rol,
                    Clinica = user.Clinica,
                    Token = token
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error interno del servidor",
                    detail = ex.Message
                });
            }
        }

        [HttpPost("GuardarUsuarios")]
        public async Task<IActionResult> GuardarUsuarios([FromBody] Usuario model)
        {
            try
            {
                // VALIDACIÓN BÁSICA
                if (string.IsNullOrWhiteSpace(model.Nombre) ||
                    string.IsNullOrWhiteSpace(model.Email) ||
                    string.IsNullOrWhiteSpace(model.Password) ||
                    string.IsNullOrWhiteSpace(model.Rol) ||
                    string.IsNullOrWhiteSpace(model.Clinica))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Todos los campos son obligatorios."
                    });
                }

                // NORMALIZAR EMAIL
                model.Email = model.Email.Trim().ToLower();

                // GENERAR CÓDIGO VERIFICACIÓN
                var codigoVerificacion =
                    new Random().Next(100000, 999999).ToString();

                var passwordHasheada =
                    BCrypt.Net.BCrypt.HashPassword(model.Password);

                var query = new StringBuilder();
                query.AppendLine(StaticResources.QueryCrearUsuarios);

                DynamicParameters parameters =
                    new DynamicParameters();

                parameters.Add("@Nombre", model.Nombre);
                parameters.Add("@Email", model.Email);
                parameters.Add("@Password", passwordHasheada);
                parameters.Add("@Rol", model.Rol);
                parameters.Add("@Clinica", model.Clinica);
                parameters.Add("@CodigoVerificacion", codigoVerificacion);
                parameters.Add("@CorreoVerificado", false);

                using var connection =
                    new System.Data.SqlClient.SqlConnection(
                        Configuration.GetConnectionString("EntitiesContext")
                    );

                var rowsAffected =
                    await connection.ExecuteAsync(
                        query.ToString(),
                        parameters
                    );

                if (rowsAffected > 0)
                {
                    Console.WriteLine("ANTES DE ENVIAR CORREO");
                    // ENVIAR CORREO SOLO SI INSERTÓ BIEN
                    await _emailService.EnviarCorreoAsync(
                        model.Email,
                        "Verificación de cuenta - sofsystem",
                        $@"
                        <div style='font-family:Arial,sans-serif;padding:20px;color:#333;'>

                            <h2 style='color:#0288d1;'>
                                Verificación de correo electrónico
                            </h2>

                            <p>
                                Hola <strong>{model.Nombre}</strong>,
                            </p>

                            <p>
                                Gracias por crear una cuenta en 
                                <strong>sofsystem</strong>.
                            </p>

                            <p>
                                Para completar el registro, ingrese el siguiente código de verificación en la aplicación:
                            </p>

                            <div style='
                                margin:30px auto;
                                width:max-content;
                                padding:15px 30px;
                                font-size:32px;
                                font-weight:bold;
                                letter-spacing:5px;
                                background:#e3f2fd;
                                color:#0288d1;
                                border-radius:12px;
                            '>
                                {codigoVerificacion}
                            </div>

                            <p>
                                Este código expirará en unos minutos por seguridad.
                            </p>

                            <p>
                                Si usted no solicitó esta cuenta, puede ignorar este mensaje.
                            </p>

                            <hr style='margin:30px 0;border:none;border-top:1px solid #ddd;'>

                            <small style='color:#777;'>
                                sofsystem • Plataforma Inteligente para Gestión Clínica
                            </small>

                        </div>"             
                    );

                    Console.WriteLine("DESPUES DE ENVIAR CORREO");

                    return Ok(new
                    {
                        success = true,
                        message = "Usuario creado correctamente. Revise su correo para verificar la cuenta."
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = "No se pudo guardar el usuario"
                });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE KEY") ||
                    ex.Message.Contains("duplicate key"))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "El correo ya existe."
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    inner = ex.InnerException?.Message,
                    detalle = ex.ToString()
                });
            }
        }


        [HttpPost("VerificarCorreo")]
        public async Task<IActionResult> VerificarCorreo( [FromBody] VerificarCorreoRequest model)
        {
            try
            {
                using var connection =
                    new SqlConnection(
                        Configuration.GetConnectionString(
                            "EntitiesContext"
                        )
                    );

                var usuario =
                    await connection.QueryFirstOrDefaultAsync<Usuario>(
                        @"SELECT *
                  FROM Usuario
                  WHERE Email = @Email
                  AND CodigoVerificacion = @Codigo",
                        new
                        {
                            Email = model.Email,
                            Codigo = model.Codigo
                        }
                    );

                if (usuario == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message =
                            "Código incorrecto."
                    });
                }

                await connection.ExecuteAsync(
                    @"UPDATE Usuario
                      SET CorreoVerificado = 1
                      WHERE Email = @Email",
                    new
                    {
                        Email = model.Email
                    }
                );

                return Ok(new
                {
                    success = true,
                    message =
                        "Correo verificado correctamente."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("test-email")]
        public async Task<IActionResult> TestEmail()
        {
            await _emailService.EnviarCorreoAsync(
                "jsolorzano_fc@hotmail.com",
                "Prueba",
                "<h1>Hola</h1>"
            );

            return Ok();
        }
    }
}