namespace ClinicaAPI.Models
{
    public class Cita
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public int DoctorId { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public DateTime FechaConfirmacion { get; set; } = DateTime.MinValue;
        public DateTime FechaCancelacion { get; set; } = DateTime.MinValue;
        public DateTime FechaRespuesta { get; set; } = DateTime.MinValue;
        public string Motivo { get; set; }
        public string Tipo { get; set; }
        public string Telefono { get; set; } 
        public string Estado { get; set; }
        public string Clinica { get; set; }
        public bool WhatsAppEnviado { get; set; } = false;
        public bool Respondida { get; set; } = false;
        public bool TokenConfirmacion { get; set; } = false;
    }
}
