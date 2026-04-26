namespace ClinicaAPI.Models
{
    public class Facturas
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public DateTime Fecha { get; set; } 
        public decimal Total { get; set; }
    }
}
