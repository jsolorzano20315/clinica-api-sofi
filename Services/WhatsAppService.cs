using System.Net.Http.Headers;

namespace ClinicaAPI.Services
{
    public class WhatsAppService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public WhatsAppService(
            IConfiguration config,
            HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        public async Task<bool> EnviarAsync(
          string telefono,
          string mensaje)
          {
            try
            {
                var instanceId = _config["UltraMsg:InstanceId"];
                var token = _config["UltraMsg:Token"];

                telefono = telefono
                    .Replace("+", "")
                    .Replace("-", "")
                    .Replace(" ", "")
                    .Trim();

                Console.WriteLine($"📲 Enviando a: {telefono}");

                var url =
                    $"https://api.ultramsg.com/{instanceId}/messages/chat";

                var body = new Dictionary<string, string>
                {
                    { "token", token },
                    { "to", telefono },
                    { "body", mensaje }
                };

                var content = new FormUrlEncodedContent(body);

                var response =
                    await _httpClient.PostAsync(url, content);

                var result =
                    await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📲 STATUS: {response.StatusCode}");
                Console.WriteLine($"📲 RESPUESTA: {result}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR WHATSAPP: {ex}");

                return false;
            }
        }
    }
}