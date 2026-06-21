using Microsoft.AspNetCore.Mvc;
using Shared.Commands;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        [HttpPost(Name = "GetWeatherForecast")]
        public IActionResult Post([FromBody] CommandTeste commandTeste)
        {
            commandTeste.ValidarCommand();
            if (commandTeste.IsValid)
            {
                return BadRequest(commandTeste.RetornarNotificacoes("Teste mensagem"));
            }

            return Ok(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray());
        }
    }
}
