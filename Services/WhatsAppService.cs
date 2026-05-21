using System.Net.Http.Headers;

namespace ClinicaAPI.Services
{
    public class WhatsAppService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        public WhatsAppService(
            IConfiguration config,
            IHttpClientFactory httpClientFactory, HttpClient httpClient)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
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

                telefono = telefono.Replace("+", "");

                var url =
                    $"https://api.ultramsg.com/{instanceId}/messages/chat";

                var client = _httpClientFactory.CreateClient();

                client.Timeout = TimeSpan.FromSeconds(30);

                var body = new Dictionary<string, string>
                    {
                        { "token", token },
                        { "to", telefono },
                        { "body", mensaje }
                    };

                var content = new FormUrlEncodedContent(body);

                var response = await client.PostAsync(url, content);

                var result = await response.Content.ReadAsStringAsync();

                Console.WriteLine(result);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}