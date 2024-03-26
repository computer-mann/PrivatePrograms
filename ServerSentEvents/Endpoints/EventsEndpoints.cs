using Carter;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServerSentEvents.Endpoints
{
    public class EventsEndpoints:CarterModule
    {
        private readonly ILogger<EventsEndpoints> logger;
        public EventsEndpoints(ILogger<EventsEndpoints> logger) : base("/privateprograms")
        {
            this.logger = logger;
        }
        public override void AddRoutes(IEndpointRouteBuilder builder)
        {
            builder.MapGet("/sse/{pathed}", async (HttpContext context, [FromRoute] string pathed) =>
            {
                context.Response.Headers.Append("Content-Type", "text/event-stream");
                logger.LogInformation("the user agent is {agent}",pathed);
                while (true)
                {
                    var json = JsonSerializer.Serialize(new { guid = Guid.NewGuid(), time = DateTime.Now, agent = pathed });
                    await context.Response.WriteAsync($"data: {json}\n\n");
                    await context.Response.Body.FlushAsync();
                    await Task.Delay(3000);
                }
            });


            builder.MapGet("/weatherforecast", (HttpContext context) =>
            {
                logger.LogInformation("the user agent is {agent}", context.Request.Headers.UserAgent.ToString());
                var summaries = new[]
           {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi();
        }

       
    }
}
