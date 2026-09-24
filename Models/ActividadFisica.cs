namespace ClinicaAPI.Models
{
    public class ActividadFisica
    {
        public int IdActividadFisica { get; set; } 
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public string? NivelActividadFisica { get; set; }
    }
}
