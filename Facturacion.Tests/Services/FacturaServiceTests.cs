
using Facturacion.ApiService.Models;
using Facturacion.ApiService.Repositories.Interfaces;
using Facturacion.ApiService.Services;
using Facturacion.ApiService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Facturacion.ApiService.Constants;
using Xunit;

namespace Facturacion.Tests.Services
{
    public class FacturaServiceTests
    {
        private readonly Mock<IFacturaRepository> _repositorioMock;
        private readonly Mock<IEmailService> _emailMock;
        private readonly Mock<ILogger<FacturaService>> _loggerMock;
        private readonly FacturaService _servicio;

        public FacturaServiceTests()
        {
            _repositorioMock = new Mock<IFacturaRepository>();
            _emailMock = new Mock<IEmailService>();
            _loggerMock = new Mock<ILogger<FacturaService>>();

            _servicio = new FacturaService(
                _repositorioMock.Object,
                _emailMock.Object,
                _loggerMock.Object);
        }

        private static Factura CrearFactura(string estado)
        {
            return new Factura
            {
                Id = "507f1f77bcf86cd799439011",
                Cliente = "Cliente de prueba",
                Correo = "prueba@example.com",
                NumeroFactura = "FAC-001",
                Valor = 150000,
                FechaVencimiento = DateTime.Today,
                Estado = estado
            };
        }

        [Fact]
        public async Task ProcesarRecordatorios_PrimerRecordatorio_CambiaASegundoRecordatorio()
        {
            var factura = CrearFactura(EstadosFactura.PrimerRecordatorio);

            _repositorioMock
                .Setup(r => r.ObtenerPendientesRecordatorioAsync())
                .ReturnsAsync(new List<Factura> { factura });

            _emailMock
                .Setup(e => e.EnviarCorreoAsync(
                    factura.Correo,
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var resultado = await _servicio.ProcesarRecordatoriosAsync();

            Assert.Equal(1, resultado);

            _emailMock.Verify(e => e.EnviarCorreoAsync(
                factura.Correo,
                It.Is<string>(a => a.Contains("Segundo recordatorio")),
                It.IsAny<string>()), Times.Once);

            _repositorioMock.Verify(r => r.ActualizarEstadoAsync(
                factura.Id!,
                EstadosFactura.SegundoRecordatorio), Times.Once);
        }

        [Fact]
        public async Task ProcesarRecordatorios_SegundoRecordatorio_CambiaADesactivado()
        {
            var factura = CrearFactura(EstadosFactura.SegundoRecordatorio);

            _repositorioMock
                .Setup(r => r.ObtenerPendientesRecordatorioAsync())
                .ReturnsAsync(new List<Factura> { factura });

            _emailMock
                .Setup(e => e.EnviarCorreoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var resultado = await _servicio.ProcesarRecordatoriosAsync();

            Assert.Equal(1, resultado);

            _repositorioMock.Verify(r => r.ActualizarEstadoAsync(
                factura.Id!,
                EstadosFactura.Desactivado), Times.Once);
        }

        [Fact]
        public async Task ProcesarRecordatorios_FacturaPagada_NoCambiaEstadoNiEnviaCorreo()
        {
            var factura = CrearFactura(EstadosFactura.Pagada);

            _repositorioMock
                .Setup(r => r.ObtenerPendientesRecordatorioAsync())
                .ReturnsAsync(new List<Factura> ());

            var resultado = await _servicio.ProcesarRecordatoriosAsync();

            Assert.Equal(0, resultado);

            _emailMock.Verify(e => e.EnviarCorreoAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Never);

            _repositorioMock.Verify(r => r.ActualizarEstadoAsync(
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ProcesarRecordatorios_SiFallaCorreo_NoActualizaEstado()
        {
            var factura = CrearFactura(EstadosFactura.PrimerRecordatorio);

            _repositorioMock
                .Setup(r => r.ObtenerPendientesRecordatorioAsync())
                .ReturnsAsync(new List<Factura> { factura });

            _emailMock
                .Setup(e => e.EnviarCorreoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ThrowsAsync(new Exception("Error simulado al enviar correo"));

            var resultado = await _servicio.ProcesarRecordatoriosAsync();

            Assert.Equal(0, resultado);

            _repositorioMock.Verify(r => r.ActualizarEstadoAsync(
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ProcesarRecordatorios_SiFallaActualizacion_NoCuentaFacturaComoProcesada()
        {
            // Arrange
            var factura = CrearFactura(EstadosFactura.PrimerRecordatorio);

            _repositorioMock
                .Setup(r => r.ObtenerPendientesRecordatorioAsync())
                .ReturnsAsync(new List<Factura> { factura });

            _emailMock
                .Setup(e => e.EnviarCorreoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _repositorioMock
                .Setup(r => r.ActualizarEstadoAsync(
                    factura.Id!,
                    EstadosFactura.SegundoRecordatorio))
                .ThrowsAsync(new InvalidOperationException(
                    "Error simulado al actualizar MongoDB"));

            // Act
            var resultado = await _servicio.ProcesarRecordatoriosAsync();

            // Assert
            Assert.Equal(0, resultado);

            _emailMock.Verify(e => e.EnviarCorreoAsync(
                factura.Correo,
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);

            _repositorioMock.Verify(r => r.ActualizarEstadoAsync(
                factura.Id!,
                EstadosFactura.SegundoRecordatorio), Times.Once);
        }
    }
}
