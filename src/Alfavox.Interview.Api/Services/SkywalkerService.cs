using Alfavox.Interview.Api.Models;
using Alfavox.Interview.Api.Exceptions;
using Newtonsoft.Json;

namespace Alfavox.Interview.Api.Services
{

    public interface ISkywalkerService
    {
        public Task<SkywalkerDetailsResponse> GetSkywalkerData();
    }

    public class SkywalkerService : ISkywalkerService
    {
        private static string ApiUrl { get; } = "https://swapi.dev/api/people/1/";

        private readonly IHypermediaService _hypermediaService;
        private readonly IHttpContextWrapper _httpContextWrapper;
        private readonly IFileService _fileService;
        private readonly ILoggingService _loggingService;

        public SkywalkerService(IHttpContextWrapper httpContextWrapper, IFileService fileService, ILoggingService loggingService, IHypermediaService hypermerdiaService)
        {
            _httpContextWrapper = httpContextWrapper;
            _fileService = fileService;
            _loggingService = loggingService;
            _hypermediaService = hypermerdiaService;
        }

        public async Task<SkywalkerDetailsResponse> GetSkywalkerData()
        {

            _loggingService.LogInformation($"Sending request to API: {ApiUrl}");

            HttpResponseMessage response = await _httpContextWrapper.GetAsync(ApiUrl);

            if (response.IsSuccessStatusCode)
            {
                _loggingService.LogInformation("API request successful.");

                string responseBody = await response.Content.ReadAsStringAsync();

                var lukeSkywalkerData = JsonConvert.DeserializeObject<SkywalkerDetailsResponse>(responseBody);

                var filmNames = await _hypermediaService.GetHypermediaData(lukeSkywalkerData.Films, "title");
                var vehicleNames = await _hypermediaService.GetHypermediaData(lukeSkywalkerData.Vehicles, "name");
                var starshipNames = await _hypermediaService.GetHypermediaData(lukeSkywalkerData.Starships, "name");

                var result = new SkywalkerDetailsResponse
                {
                    Films = filmNames,
                    Vehicles = vehicleNames,
                    Starships = starshipNames
                };

                SaveResponseToFile(result, "C:\\Users\\Albert\\Desktop");

                return result;
            }

            else
            {
                _loggingService.LogError($"API request failed with status {response.StatusCode}");

                throw new BadRequestException($"API request failed with status {response.StatusCode}");
            }
        }

        private void SaveResponseToFile(SkywalkerDetailsResponse response, string path)
        {
            _loggingService.LogInformation("Saving response to file");

            string jsonResult = JsonConvert.SerializeObject(response);

            string filePath = Path.Combine(path, "skywalkerDetails.txt");

            _fileService.WriteAllText(filePath, jsonResult);

            _loggingService.LogInformation("Reponse saved in file");
        }
    }
}
