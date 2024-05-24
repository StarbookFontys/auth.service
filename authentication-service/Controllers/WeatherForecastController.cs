using authentication_service.DAL.Caching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace authentication_service.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private CacheManager<string> cacheManager;
		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"

		};

		public WeatherForecastController(ILogger<WeatherForecastController> logger, CacheManager<string> _cacheManager)
		{
			cacheManager = _cacheManager;
		}
		[HttpGet(Name = "GetWeatherForecast")]
		public async Task<IActionResult> Get()
		{
			return Ok("I have no mouth but I must scream.");
		}

		[HttpPost("{Key}")]
		public async Task<IActionResult> Post(string key)
		{
			cacheManager.GetOrCreate(key,() => CreateUserValue());
			return Ok();
		}

		private string CreateUserValue()
		{
			return "UserValue";
		}
	}
}
