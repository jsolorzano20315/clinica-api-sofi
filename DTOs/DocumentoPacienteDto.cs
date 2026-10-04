namespace ClinicaAPI.DTOs
{
    public class DocumentoPacienteDto
    {
        public int IdArchivoImagen { get; set; }

        public int IdImagen { get; set; }

        public string? NombreArchivo { get; set; }

        public string? TipoArchivo { get; set; }

        public string? RutaArchivo { get; set; }

        public DateTime Fecha { get; set; }
    }
}