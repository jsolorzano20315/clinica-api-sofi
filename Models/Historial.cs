namespace ClinicaAPI.Models
{
    public class Historial
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public DateTime Fecha { get; set; }
        public string Nota { get; set; } 
    }
}
