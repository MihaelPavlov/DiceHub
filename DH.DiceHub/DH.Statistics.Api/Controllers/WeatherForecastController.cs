using DH.Messaging.HttpClient.UserContext;
using DH.Statistics.Application;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DH.Statistics.Api.Controllers
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
        private readonly IB2bUserContext _userContext;
        readonly IMediator mediator;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IB2bUserContext userContext, IMediator mediator)
        {
            this.mediator = mediator;
            _logger = logger;
            _userContext = userContext;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            await this.mediator.Send(new CreateStatisticCommand(), CancellationToken.None);
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
