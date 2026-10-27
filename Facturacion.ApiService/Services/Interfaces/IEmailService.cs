namespace Facturacion.ApiService.Services.Interfaces
{
    public interface IEmailService
    {
        Task EnviarCorreoAsync(
            string destinatario,
            string asunto,
            string mensaje);
    }
}
