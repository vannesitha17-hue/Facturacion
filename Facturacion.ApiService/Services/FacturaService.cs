using Facturacion.ApiService.Repositories;
using Facturacion.ApiService.Services.Interfaces;
using Facturacion.ApiService.Repositories.Interfaces;
using Facturacion.ApiService.Constants;

namespace Facturacion.ApiService.Services
{
    public class FacturaService
    {
        private readonly IFacturaRepository _facturaRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<FacturaService> _logger;

        public FacturaService(
            IFacturaRepository facturaRepository,
            IEmailService emailService,
            ILogger<FacturaService> logger)
        {
            _facturaRepository = facturaRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<int> ProcesarRecordatoriosAsync()
        {
            var facturasPendientes = await _facturaRepository.ObtenerPendientesRecordatorioAsync();

            int procesadas = 0;

            foreach (var factura in facturasPendientes)
            {
                try
                {
                    string nuevoEstado;
                    string asunto;
                    string mensaje;

                    if (factura.Estado == EstadosFactura.PrimerRecordatorio)
                    {
                        nuevoEstado = EstadosFactura.SegundoRecordatorio;
                        asunto = $"Segundo recordatorio - Factura {factura.NumeroFactura}";
                        mensaje = $"Estimado/a {factura.Cliente}, " +
                                  $"le recordamos que la factura {factura.NumeroFactura} " +
                                  $"por valor de {factura.Valor:C} está pendiente de pago.";
                    }
                    else
                    {
                        nuevoEstado = EstadosFactura.Desactivado;
                        asunto = $"Aviso de desactivación - Factura {factura.NumeroFactura}";
                        mensaje = $"Estimado/a {factura.Cliente}, " +
                                  $"le informamos que la factura {factura.NumeroFactura} " +
                                  "continúa pendiente y se procederá con la desactivación.";
                    }

                    await _emailService.EnviarCorreoAsync(
                        factura.Correo,
                        asunto,
                        mensaje);

                    if (string.IsNullOrWhiteSpace(factura.Id))
                    {
                        _logger.LogWarning(
                            "La factura {NumeroFactura} no tiene un ID válido.",
                            factura.NumeroFactura);

                        continue;
                    }

                    await _facturaRepository.ActualizarEstadoAsync(
                        factura.Id,
                        nuevoEstado);

                    _logger.LogInformation(
                        "Factura {NumeroFactura} procesada. Nuevo estado: {Estado}",
                        factura.NumeroFactura,
                        nuevoEstado);

                    procesadas++;

                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error al procesar la factura {NumeroFactura}",
                        factura.NumeroFactura);
                }
            }

            return procesadas;
        }
    }
}
