namespace ClinicaAPI.Models
{
    public class Paciente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime Fecha { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Clinica { get; set; } 
    }
}
