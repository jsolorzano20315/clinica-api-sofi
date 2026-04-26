namespace ClinicaAPI.DTOs
{
    public class DoctoresDto
    {
        public int Id { get; set; }
        public string NombreDoctor { get; set; } 
        public int EspecialidadId { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string NombreEspecialida { get; set; } 
    }
}
