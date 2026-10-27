using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Facturacion.ApiService.Models
{
    public class Factura
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("cliente")]
        public string Cliente { get; set; } = string.Empty;

        [BsonElement("correo")]
        public string Correo { get; set; } = string.Empty;

        [BsonElement("numeroFactura")]
        public string NumeroFactura { get; set; } = string.Empty;

        [BsonElement("valor")]
        public decimal Valor { get; set; }

        [BsonElement("fechaVencimiento")]
        public DateTime FechaVencimiento { get; set; }

        [BsonElement("estado")]
        public string Estado { get; set; } = string.Empty;
    }
}
