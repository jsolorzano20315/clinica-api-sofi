namespace ClinicaAPI.Models
{
    public class Alergias
    {
        public int IdAlergia { get; set; }
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public string? EstadoAlergia { get; set; }
        public string? Alergia { get; set; }

    }
}
