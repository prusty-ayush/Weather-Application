
using Weather.Domain.Entities;

namespace Weather.Domain.Interfaces
{
    public interface IWeatherDataRepository
    {
        public Task Save(WeatherData data);
        //Task<List<WeatherData>> GetByUsername(string username);
    }
}
