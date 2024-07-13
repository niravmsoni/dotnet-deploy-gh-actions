using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Starter.WebAPI.Controllers;

namespace Starter.WebAPI.UnitTests
{
    public class WeatherForecastControllerTests
    {
        private readonly WeatherForecastController _controller;
        private readonly Mock<ILogger<WeatherForecastController>> _loggerMock;

        public WeatherForecastControllerTests()
        {
            _loggerMock = new Mock<ILogger<WeatherForecastController>>();
            _controller = new WeatherForecastController(_loggerMock.Object);
        }

        [Fact]
        public void Get_ReturnsFiveWeatherForecasts()
        {
            // Act
            var forecasts = _controller.Get();

            // Assert
            forecasts.Should().NotBeNull();
            forecasts.Should().HaveCount(5);
        }
    }
}