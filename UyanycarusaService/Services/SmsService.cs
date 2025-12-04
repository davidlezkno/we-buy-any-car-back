using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;

namespace UyanycarusaService.Services
{
    /// <summary>
    /// Servicio para operaciones de SMS
    /// </summary>
    public class SmsService : ISmsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SmsService> _logger;
        private readonly ITokenService _tokenService;

        public SmsService(
            IHttpClientFactory httpClientFactory,
            ILogger<SmsService> logger,
            IConfiguration configuration,
            ITokenService tokenService)
        {
            _httpClient = httpClientFactory.CreateClient("WebuyAnyCarApi");
            _logger = logger;
            _tokenService = tokenService;
        }

        /// <inheritdoc />
        public async Task<JsonElement> SendSmsAsync(JsonElement model)
        {
            try
            {
                _logger.LogInformation("Enviando SMS a través del servicio externo /Sms/send");
                var accessToken = await _tokenService.GetAccessTokenAsync();
                var request = new HttpRequestMessage(HttpMethod.Post, "/Sms/send")
                {
                    Content = JsonContent.Create(model)
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                using var response = await _httpClient.SendAsync(request);

                var content = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    var json = JsonSerializer.Deserialize<JsonElement>(content);
                    return json;
                }

                _logger.LogWarning("El servicio externo /Sms/send retornó un código de estado: {StatusCode}", response.StatusCode);

                throw new HttpRequestException(
                    $"Error al enviar SMS. StatusCode: {response.StatusCode}, Detail: {content}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error al comunicarse con el servicio externo /Sms/send");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al enviar SMS");
                throw;
            }
        }
    }
}
