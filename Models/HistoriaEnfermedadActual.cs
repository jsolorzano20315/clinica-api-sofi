namespace ClinicaAPI.Models
{
    public class HistoriaEnfermedadActual
    {
        public int IdHEA { get; set; }
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public string? HistoriaEnfermedad { get; set; }
    }
}
