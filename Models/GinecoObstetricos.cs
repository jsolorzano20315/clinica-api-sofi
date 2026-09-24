namespace ClinicaAPI.Models
{
    public class GinecoObstetricos
    {
        public int IdAlergia { get; set; }
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public int? Gestaciones { get; set; }
        public int? Partos { get; set; }
        public int? Cesareas { get; set; }
        public int? Abortos { get; set; }
        public int? HijosVivos { get; set; }
        public int? HijosMuertos { get; set; }
    }
}
