namespace ClinicaAPI.Models
{
    public class AntecedentesPersonales
    {
        public int IdAntecedente { get; set; }
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public string? AntecedentesPersona { get; set; } 
    }
}
