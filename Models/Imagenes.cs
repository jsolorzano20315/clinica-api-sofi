namespace ClinicaAPI.Models
{
    public class Imagenes
    {

        public int IdImagen { get; set; }
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public string? EstudiosImagen { get; set; }
    }
}
