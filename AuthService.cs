// Services/AuthService.cs
using Azure.Core;
using ClinicaAPI.Models;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaAPI.Services
{
    public class AuthService
    {
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        // Validar usuario (simulado en memoria)
        public Usuario? ValidateUser(string nombre, string rol, string email, string password, string clinica) 
        {
            var usuarios = new List<Usuario>
            { 
                new Usuario { Nombre = nombre, Email = email, Rol = rol, Password = password, Clinica = clinica},
            };

            return usuarios.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password
            );
        }

        // Generar JWT para el usuario
        public string GenerateToken(Usuario user)
        {
            // Obtener configuración JWT
            var jwtSettings = _config.GetSection("Jwt");
            var keyString = jwtSettings["Key"];

            if (string.IsNullOrWhiteSpace(keyString))
                throw new Exception("JWT Key no configurada en appsettings.json");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("rol", user.Rol),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}