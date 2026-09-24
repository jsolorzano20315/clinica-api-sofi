namespace ClinicaAPI.Models
{
    public class ImpresionDiagnostica
    {
        public int IdImpresionDiagnostica { get; set; }
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Diagnostica { get; set; }  
    }
}
