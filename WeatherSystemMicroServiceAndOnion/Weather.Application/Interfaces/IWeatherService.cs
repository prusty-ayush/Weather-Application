using Weather.Application.DTO;

namespace Weather.Application.Interfaces
{
    public interface IWeatherService
    {
        public Task<WeatherDto> GetWeather(string city);
    }
}
