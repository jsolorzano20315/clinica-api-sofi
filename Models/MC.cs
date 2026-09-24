namespace ClinicaAPI.Models
{
    public class MC
    {
        public int IdMC { get; set; }
        public int? IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Estatura { get; set; }
        public decimal? IndiceMasaCorporal { get; set; } 
    }
}
