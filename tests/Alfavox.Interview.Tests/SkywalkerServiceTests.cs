using Alfavox.Interview.Api.Models;
using Alfavox.Interview.Api.Services;
using Alfavox.Interview.Infrastructure;
using Moq;
using Newtonsoft.Json;
using System.Net;
using Xunit;

public class SkywalkerServiceTests
{
    [Fact]
    public async Task GetSkywalkerData_SuccessfulRequest_ReturnsSkywalkerDetailsResponse()
    {
        // Arrange
        var expectedResponse = new SkywalkerDetailsResponse
        {
            Films = new List<string> { "A New Hope", "The Empire Strikes Back", "Return of the Jedi", "Revenge of the Sith" },
            Vehicles = new List<string> { "Snowspeeder", "Imperial Speeder Bike" },
            Starships = new List<string> { "X-wing", "Imperial shuttle" }
        };

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(expectedResponse))
        };

        var mockHttpClientWrapper = new Mock<IHttpClientWrapper>();
        var mockLoggingService = new Mock<ILoggingService>();

        mockHttpClientWrapper.Setup(wrapper => wrapper.GetAsync("http://www.test.com")).ReturnsAsync(httpResponseMessage);

        var skywalkerService = new SkywalkerService(
            mockLoggingService.Object,
            mockHttpClientWrapper.Object
        );

        // Act
        var result = await skywalkerService.GetSkywalkerData();

        // Asserts
        Assert.NotNull(result);

        Assert.NotNull(result.Films);
        Assert.Equal(expectedResponse.Films, result.Films);

        Assert.NotNull(result.Vehicles);
        Assert.Equal(expectedResponse.Vehicles, result.Vehicles);

        Assert.NotNull(result.Starships);
        Assert.Equal(expectedResponse.Starships, result.Starships);
    }
}