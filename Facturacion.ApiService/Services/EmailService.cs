using Facturacion.ApiService.Services.Interfaces;

namespace Facturacion.ApiService.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task EnviarCorreoAsync(
            string destinatario,
            string asunto,
            string mensaje)
        {
            _logger.LogInformation(
                "Correo simulado. Destinatario: {Destinatario}. " +
                "Asunto: {Asunto}. Mensaje: {Mensaje}",
                destinatario,
                asunto,
                mensaje);

            return Task.CompletedTask;
        }
    }
}