namespace ClinicaAPI.Models
{
    public class ArchivosImagenes
    {
        public int IdArchivoImagen { get; set; }
        public int IdImagen { get; set; }
        public int IdPaciente { get; set; }
        public string? Clinica { get; set; }
        public DateTime? Fecha { get; set; }
        public string? NombreArchivo { get; set; }
        public string? RutaArchivo { get; set; }
    }
}
