using ClinicaAPI.Models;
using System.Text;
using ClinicaAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AuthService _authService;
        public UsuariosController(IConfiguration configuration, AuthService authService) 
        {
            Configuration = configuration;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IConfiguration Configuration { get; }


    }
}
