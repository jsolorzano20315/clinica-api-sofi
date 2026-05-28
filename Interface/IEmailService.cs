namespace ClinicaAPI.Interface
{
    public interface IEmailService
    {
        Task EnviarCorreoAsync(string destino, string asunto, string mensaje);
    }
}
