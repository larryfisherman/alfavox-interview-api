using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Alfavox.Interview.Api.Exceptions;
using Alfavox.Interview.Api.Models;
using Alfavox.Interview.Api.Services;
using Moq;
using Newtonsoft.Json;
using Xunit;

public class SkywalkerServiceTests
{
    [Fact]
    public async Task GetSkywalkerData_SuccessfulRequest_ReturnsSkywalkerDetailsResponse()
    {
        var apiUrl = "https://swapi.dev/api/people/1/";
        var expectedResponse = new SkywalkerDetailsResponse
        {
            Films = new List<string> { "Film1", "Film2" },
            Vehicles = new List<string> { "Vehicle1", "Vehicle2" },
            Starships = new List<string> { "Starship1", "Starship2" }
        };

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(expectedResponse))
        };

        var mockHttpContextWrapper = new Mock<IHttpContextWrapper>();
        var mockHypermediaService = new Mock<IHypermediaService>();
        var mockLoggingService = new Mock<ILoggingService>();
        var fileServiceMock = new Mock<IFileService>();

        mockHttpContextWrapper.Setup(wrapper => wrapper.GetAsync(apiUrl)).ReturnsAsync(httpResponseMessage);

        mockHypermediaService.SetupSequence(x => x.GetHypermediaData(It.IsAny<List<string>>(), It.IsAny<string>()))
            .ReturnsAsync(new List<string> { "Film1", "Film2" })
            .ReturnsAsync(new List<string> { "Vehicle1", "Vehicle2" })
            .ReturnsAsync(new List<string> { "Starship1", "Starship2" });

        var skywalkerService = new SkywalkerService(
            mockHttpContextWrapper.Object,
            fileServiceMock.Object,
            mockLoggingService.Object,
            mockHypermediaService.Object
        );

        var result = await skywalkerService.GetSkywalkerData();

        // Assert
        Assert.NotNull(result.Films);
        Assert.Equal(expectedResponse.Films, result.Films);


        Assert.NotNull(result.Vehicles);
        Assert.Equal(expectedResponse.Vehicles, result.Vehicles);


        Assert.NotNull(result.Starships);
        Assert.Equal(expectedResponse.Starships, result.Starships);

    }

    [Fact]
    public async Task GetSkywalkerData_UnsuccessfulRequest_ThrowsBadRequestException()
    {
        // Arrange
        var mockLoggingService = new Mock<ILoggingService>();
        var mockHttpContextWrapper = new Mock<IHttpContextWrapper>();
        var mockHypermediaService = new Mock<IHypermediaService>();
        var fileServiceMock = new Mock<IFileService>();


        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        mockHttpContextWrapper.Setup(wrapper => wrapper.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponseMessage);

        var skywalkerService = new SkywalkerService(mockHttpContextWrapper.Object, fileServiceMock.Object, mockLoggingService.Object,  mockHypermediaService.Object);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => skywalkerService.GetSkywalkerData());
    }
}