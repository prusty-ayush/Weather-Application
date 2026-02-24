using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Weather.Application.DTO;
using Weather.Application.Interfaces;
using Weather.Domain.Interfaces;


namespace Weather.Infrastructure.Services
{
    public class WeatherService :IWeatherService
    {
        private readonly HttpClient httpclient;
        private readonly IConfiguration config;
        private readonly IDistributedCache cache;
        private readonly ILogger<WeatherService> logger;

        public WeatherService(HttpClient _httpclient, IConfiguration _config, IDistributedCache _cache, ILogger<WeatherService> _logger)
        {
            httpclient = _httpclient;
            config = _config;
            cache = _cache;
            logger = _logger;
        }

        public async Task<WeatherDto> GetWeather(string city)
        {
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("Invalid city name");

            var cacheKey = $"Weather:{city.ToLower()}";
            var cached = await cache.GetStringAsync(cacheKey);
            if(cached != null){
                logger.LogInformation($"Cache hit for city: {city}");
                return JsonSerializer.Deserialize<WeatherDto>(cached)!;
            }

            logger.LogInformation($"Cache miss for city: {city} - calling API");

            //Api Details Fetching
            var WeatherApiDetails = config.GetSection("WeatherApi") ?? throw new InvalidOperationException("Weather Api Section is Missing");
            var ApiKey = WeatherApiDetails["Key"] ?? throw new InvalidOperationException("Weather Api Key is Missing");
            var BaseUrl = WeatherApiDetails["BaseUrl"] ?? throw new InvalidOperationException("Weather Api Base Url is Missing");

            //final url
            var url = $"{BaseUrl}?q={city}&appid={ApiKey}&units=metric";


            //Response from Api

            var whole_response = await httpclient.GetAsync(url);

            //Check the response is valid or not
            if (!whole_response.IsSuccessStatusCode)
            {
                logger.LogError("Weather Api call request failed");
                throw new Exception("Weather Api Request Failed");
            }
            logger.LogInformation($"Weather Api call successfull for city: {city}");
            //getting the information(json) from response
            var information_from_response = await whole_response.Content.ReadAsStringAsync();

            //parsing of the json
            using var data = JsonDocument.Parse(information_from_response);

            //Getting root elements
            var root_elements = data.RootElement;

            //returning only the necessary data
            WeatherDto weatherDto =  new WeatherDto
            {
                City_Name = root_elements.GetProperty("name").GetString() ?? city,
                Temperature = root_elements.GetProperty("main").GetProperty("temp").GetDouble(),
                Humidity = root_elements.GetProperty("main").GetProperty("humidity").GetDouble(),
                Latitude = root_elements.GetProperty("coord").GetProperty("lat").GetDecimal(),
                Longitude = root_elements.GetProperty("coord").GetProperty("lon").GetDecimal(),
            };

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            };
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(weatherDto), cacheOptions);
            logger.LogInformation($"Weather data cached for {city} - expires in 2 minutes");
            return weatherDto;
        }
    }
}