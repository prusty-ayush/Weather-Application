using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Weather.Domain.Entities
{
    public class WeatherData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Username { get; set; } = string.Empty;
        public string City_Name { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
