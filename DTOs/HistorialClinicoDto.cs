namespace ClinicaAPI.DTOs
{
    public class HistorialClinicoDto
    {
        public string Motivos { get; set; } 
        public string Telefono { get; set; }
        public string NombrePaciente { get; set; }
        public DateTime UltimaFechaCita { get; set; } 
        public int TotalCitas { get; set; }
        public string Clinica { get; set; }
    }
}
