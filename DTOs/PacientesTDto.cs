using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClinicaAPI.DTOs
{
    public class PacientesTDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public int PacienteId { get; set; }
        public DateTime Fecha { get; set; }
        public string Motivo { get; set; }
    }
}
