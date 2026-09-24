namespace ClinicaAPI.Models
{
    public class ECG
    {
        public int IdECG { get; set; }
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public string? InterpretacionElectrocardiograma { get; set; } 
    }
}
