using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weather.Application.Interfaces;
using Weather.Domain.Entities;
using Weather.Domain.Interfaces;

namespace Weather.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService weatherservice;
        private readonly IWeatherDataRepository weatherRepository;
        private readonly ILogger<WeatherController> logger;
        public WeatherController(IWeatherService _weatherservice, IWeatherDataRepository _weatherRepository, ILogger<WeatherController> _logger)
        {
            weatherservice = _weatherservice;
            weatherRepository = _weatherRepository;
            logger = _logger;
        }

        [HttpGet("{city}")]
        public async Task<IActionResult> GetWeatherData(string city)
        {
            string username = User.Identity?.Name ?? "Unknown";
            logger.LogInformation($"User {username} requested weather for {city}" );
            try
            {
                var response = await weatherservice.GetWeather(city);
                logger.LogInformation($"Weather fetched successfully for {city}");
                await weatherRepository.Save(new WeatherData
                {
                    Username = username,
                    City_Name = response.City_Name?? city,
                    Temperature = response.Temperature,
                    Humidity = response.Humidity,
                    Latitude = response.Latitude,
                    Longitude = response.Longitude,

                });
                return Ok(response);
            }
            catch (Exception)
            {
                logger.LogError($"Error in fetching weather for city: {city}");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }

        }
    }
}
