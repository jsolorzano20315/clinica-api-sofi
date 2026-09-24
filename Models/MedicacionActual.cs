namespace ClinicaAPI.Models
{
    public class MedicacionActual
    {
        public int IdMedicacion { get; set; }
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Medicacion  { get; set; } 
        
    }
}
