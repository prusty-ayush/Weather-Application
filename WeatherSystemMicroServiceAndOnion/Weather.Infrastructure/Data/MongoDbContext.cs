using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Weather.Domain.Entities;

namespace Weather.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase db;

        public MongoDbContext(IConfiguration config)
        {
            var connectionString = config["MongoDB:ConnectionString"];
            var dbname = config["MongoDB:DatabaseName"];
            var client = new MongoClient(connectionString);
            db = client.GetDatabase(dbname);
        }

        public IMongoCollection<WeatherData> weatherdata =>db.GetCollection<WeatherData>("weatherdata");
    }
}
