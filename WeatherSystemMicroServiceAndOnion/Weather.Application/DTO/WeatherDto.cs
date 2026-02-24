using System;
using System.Collections.Generic;
using System.Text;

namespace Weather.Application.DTO
{
    public class WeatherDto
    {
        public string? City_Name { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
