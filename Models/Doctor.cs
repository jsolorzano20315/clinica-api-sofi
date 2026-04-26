namespace ClinicaAPI.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int EspecialidadId { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
    } 
}
