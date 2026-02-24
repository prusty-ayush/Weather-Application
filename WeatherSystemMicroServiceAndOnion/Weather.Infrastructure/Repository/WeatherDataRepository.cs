using Weather.Domain.Entities;
using Weather.Domain.Interfaces;
using Weather.Infrastructure.Data;

namespace Weather.Infrastructure.Repository
{
    public class WeatherDataRepository : IWeatherDataRepository
    {
        private readonly MongoDbContext db;

        public WeatherDataRepository(MongoDbContext _db)
        {
            db = _db;
        }

        public async Task Save(WeatherData data)
        {
            await db.weatherdata.InsertOneAsync(data);
        }
    }
}
