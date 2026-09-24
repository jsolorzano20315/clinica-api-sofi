namespace ClinicaAPI.Models
{
    public class RiesgoCardiovascular
    {
        public int IdROAS { get; set; }
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public string? ResultadoEvaluacion { get; set; } 
    }
}
