using System.Text.Json;

namespace UyanycarusaService.Services
{
    /// <summary>
    /// Interfaz para el servicio de SMS
    /// </summary>
    public interface ISmsService
    {
        /// <summary>
        /// Envía un mensaje SMS
        /// </summary>
        /// <param name="model">Datos del mensaje SMS</param>
        /// <returns>Respuesta del envío de SMS como JSON</returns>
        Task<JsonElement> SendSmsAsync(JsonElement model);
    }
}
