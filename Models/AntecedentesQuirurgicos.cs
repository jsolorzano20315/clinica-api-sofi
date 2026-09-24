namespace ClinicaAPI.Models
{
    public class AntecedentesQuirurgicos
    {
        public int IdAntecedenteQuirurgico { get; set; }
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public string? AntecedentesQuirurgico { get; set; } 
       
    }
}
