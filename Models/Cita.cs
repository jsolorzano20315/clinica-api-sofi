namespace ClinicaAPI.Models
{
    public class Cita
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public int DoctorId { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaConfirmacion { get; set; }
        public DateTime FechaCancelacion { get; set; }
        public DateTime FechaRespuesta { get; set; } 
        public string Motivo { get; set; }
        public string Tipo { get; set; }
        public string Telefono { get; set; } 
        public string Estado { get; set; }
        public string Clinica { get; set; }
        public Boolean WhatsAppEnviado { get; set; }
        public Boolean Respondida { get; set; }
        public Boolean TokenConfirmacion { get; set; } 
    }
}
