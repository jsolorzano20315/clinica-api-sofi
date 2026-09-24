namespace ClinicaAPI.Models
{
    public class Laboratorios
    {
        public int IdLaboratorio { get; set; }
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public string? ResultadosLaboratorio { get; set; }
    }
}
