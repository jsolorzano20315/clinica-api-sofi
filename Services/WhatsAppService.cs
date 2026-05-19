using System.Net.Http.Headers;

namespace ClinicaAPI.Services
{
    public class WhatsAppService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public WhatsAppService(
            IConfiguration config,
            IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public async Task EnviarAsync(string telefono, string mensaje)
        {
            var instanceId = _config["UltraMsg:InstanceId"];
            var token = _config["UltraMsg:Token"];

            telefono = telefono.Replace("+", "");

            var url =
                $"https://api.ultramsg.com/{instanceId}/messages/chat";

            var client = _httpClientFactory.CreateClient();

            var body = new Dictionary<string, string>
            {
                { "token", token },
                { "to", telefono },
                { "body", mensaje }
            };

            var content = new FormUrlEncodedContent(body);

            await client.PostAsync(url, content);
        }
    }
}