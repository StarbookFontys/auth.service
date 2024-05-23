using authentication_service.DAL.Caching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace authentication_service.Controllers
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
		[HttpGet(Name = "GetWeatherForecast")]
		public async Task<IActionResult> Get()
		{
			return Ok("I have no mouth but I must scream.");
		}

		[HttpGet("/{Value}")]
		public async Task<IActionResult> Get(string Value)
		{
			CacheManager cacheManager = new DAL.Caching.CacheManager();
			cacheManager.AddCache();
			DAL.Caching.CacheModels.CacheUserModel ValueReturned = cacheManager.ReadCache();
			return Ok(ValueReturned);
		}
	}
}
