namespace ClinicaAPI.Models
{
    public class ROAS
    {
        public int IdROAS { get; set; }
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public string? RevisionAparatosSistemas { get; set; }

    }
}
