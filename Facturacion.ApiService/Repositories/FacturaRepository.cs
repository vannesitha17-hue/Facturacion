
using Facturacion.ApiService.Models;
using MongoDB.Driver;
using Facturacion.ApiService.Repositories.Interfaces;

namespace Facturacion.ApiService.Repositories
{
    public class FacturaRepository : IFacturaRepository
    {
        private readonly IMongoCollection<Factura> _facturas;

        public FacturaRepository(IMongoDatabase database)
        {
            _facturas = database.GetCollection<Factura>("Facturas");
        }

        public async Task<List<Factura>> ObtenerTodasAsync()
        {
            return await _facturas
                .Find(_ => true)
                .ToListAsync();
        }

        public async Task<List<Factura>> ObtenerPorEstadoAsync(string estado)
        {
            return await _facturas
                .Find(f => f.Estado == estado)
                .ToListAsync();
        }

        public async Task ActualizarEstadoAsync(string id, string nuevoEstado)
        {
            var filtro = Builders<Factura>.Filter.Eq(f => f.Id, id);

            var actualizacion = Builders<Factura>.Update
                .Set(f => f.Estado, nuevoEstado);

            await _facturas.UpdateOneAsync(filtro, actualizacion);
        }
    }
}
