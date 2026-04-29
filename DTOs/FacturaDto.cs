namespace ClinicaAPI.DTOs
{
    public class FacturaDto 
    {
        public int Id { get; set; }
        public string Paciente { get; set; }
        public string Motivo { get; set; } 
        public DateTime Fecha { get; set; }
        public decimal PrecioUnitario { get; set; }

    }
}
