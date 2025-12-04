using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UyanycarusaService.Services;

namespace UyanycarusaService.Controllers
{
    /// <summary>
    /// Controlador para operaciones de SMS
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [Authorize]
    public class SmsController : ControllerBase
    {
        private readonly ISmsService _smsService;
        private readonly ILogger<SmsController> _logger;

        public SmsController(
            ISmsService smsService,
            ILogger<SmsController> logger)
        {
            _smsService = smsService;
            _logger = logger;
        }

        /// <summary>
        /// Envía un mensaje SMS
        /// </summary>
        /// <param name="model">Datos del mensaje SMS</param>
        /// <returns>Respuesta del envío de SMS del servicio externo</returns>
        /// <response code="200">SMS enviado correctamente</response>
        /// <response code="400">Solicitud inválida para el servicio externo</response>
        /// <response code="401">No autorizado. Se requiere un token JWT válido</response>
        /// <response code="500">Error al consumir el servicio externo</response>
        [HttpPost("send")]
        [ProducesResponseType(typeof(JsonElement), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonElement>> SendSms([FromBody] JsonElement model)
        {
            try
            {
                var result = await _smsService.SendSmsAsync(model);
                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al comunicarse con el servicio externo de SMS",
                    detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al enviar SMS");
                return StatusCode(500, new
                {
                    message = "Error inesperado al procesar la solicitud de envío de SMS",
                    detail = ex.Message
                });
            }
        }
    }
}
