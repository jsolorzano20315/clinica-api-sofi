using Microsoft.AspNetCore.Mvc;

namespace ClinicaAPI.Controllers
{
    public class FacturacionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
