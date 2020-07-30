using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using zipkin.Models;

namespace zipkin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{days:int}")]
        public IEnumerable<WeatherForecast> GenerateWeatherData(int days)
        {
            // logger.Info($"Generating data for {days} days");
            _logger.LogInformation($"Generating data for {days} days");

            var rng = new Random();
            return Enumerable
                .Range(1, days)
                .Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> GenerateWeatherData() {
            return GenerateWeatherData(5);
        }
    }
}
