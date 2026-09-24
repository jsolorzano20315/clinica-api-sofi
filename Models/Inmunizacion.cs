namespace ClinicaAPI.Models
{
    public class Inmunizacion
    {
        public int IdInmunizacion { get; set; }
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public string? EstadoInmunizacion { get; set; } 
         
    }
}
