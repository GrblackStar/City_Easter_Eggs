using Microsoft.AspNetCore.Mvc;

namespace REST_Test_Project.Controllers
{
	[ApiController]
	[Route("[controller]/[action]")]
	public class PepegaController : ControllerBase
	{
		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<PepegaController> _logger;

		public PepegaController(ILogger<PepegaController> logger)
		{
			_logger = logger;
		}

		public IEnumerable<WeatherForecast> GetPoints()
		{
			return Enumerable.Range(1, 5).Select(index => new WeatherForecast
			{
				Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
				TemperatureC = Random.Shared.Next(-20, 55),
				Summary = Summaries[Random.Shared.Next(Summaries.Length)]
			})
			.ToArray();
		}
	}
}
