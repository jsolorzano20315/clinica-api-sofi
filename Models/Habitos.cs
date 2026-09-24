namespace ClinicaAPI.Models
{
    public class Habitos
    {
        public int IdHabito { get; set; }
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public string? DescripcionHabitos { get; set; }
        
    }
}
