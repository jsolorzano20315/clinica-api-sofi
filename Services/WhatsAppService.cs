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
                    .Trim();

                var url =
                    $"https://api.ultramsg.com/{instanceId}/messages/chat";

                _httpClient.Timeout = TimeSpan.FromSeconds(30);

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

                Console.WriteLine(result);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}