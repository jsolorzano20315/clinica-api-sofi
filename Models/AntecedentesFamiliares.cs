namespace ClinicaAPI.Models
{
    public class AntecedentesFamiliares
    {
        public int IdAntecedenteFamiliar { get; set; }
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public string? AntecedentesFamilia { get; set; }
    }
}
