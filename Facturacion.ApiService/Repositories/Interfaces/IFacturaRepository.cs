
using Facturacion.ApiService.Models;

namespace Facturacion.ApiService.Repositories.Interfaces
{
    public interface IFacturaRepository
    {
        Task<List<Factura>> ObtenerTodasAsync();

        Task<List<Factura>> ObtenerPorEstadoAsync(string estado);

        Task ActualizarEstadoAsync(string id, string nuevoEstado);
    }
}
