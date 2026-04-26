namespace ClinicaAPI.DTOs
{
    public class PacienteDto
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } 
        public DateTime FechaNacimiento { get; set; }
        public DateTime Fecha { get; set; } 
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Clinica { get; set; } 
    }
}
