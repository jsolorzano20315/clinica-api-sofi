using ClinicaAPI.DTOs;
using ClinicaAPI.Models;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace ClinicaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(IConfiguration configuration, AuthService authService)
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginResponse request)
        {
            // Validación básica del request
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Email y contraseña son obligatorios" });
            }

            var userName = User.Identity?.Name ?? "Anonimo";

            List<LoginResponse> result;
            var query = new StringBuilder();

            query.AppendLine(StaticResources.QueryUserLogin);

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Email", request.Email);
            parameters.Add("@Password", request.Password);

            using var connection = new System.Data.SqlClient.SqlConnection(Configuration.GetConnectionString("EntitiesContext"));
            result = (await connection.QueryAsync<LoginResponse>(query.ToString(), parameters)).ToList();

            //return Ok(result ?? new List<LoginRequest>());


            //Validar usuario
            var userDb = result.FirstOrDefault();

            if (userDb == null)
            {
                return Unauthorized();
            }

            var user = _authService.ValidateUser(
                userDb.Nombre,
                userDb.Rol,
                userDb.Email,
                userDb.Password,
                userDb.Clinica
            );

            // Generar token JWT
            string token;
            try
            {
                token = _authService.GenerateToken(user);
            }
            catch (Exception ex)
            {
                // Si algo falla al generar token
                return StatusCode(500, new { message = "Error al generar token", detail = ex.Message });
            }

            // Retornar respuesta
            return Ok(new LoginResponse
            {
                Nombre = user.Nombre,
                Email = user.Email,
                Rol = user.Rol,
                Clinica = user.Clinica,
                Token = token
            });
        }
    }
}