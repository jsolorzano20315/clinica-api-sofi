namespace ClinicaAPI.Models
{
    public class VerificarCorreoRequest
    {
        public string Email { get; set; } = string.Empty;

        public string Codigo { get; set; } = string.Empty;
    }
}
