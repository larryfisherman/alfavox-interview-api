using Alfavox.Interview.Api.Models;
using Alfavox.Interview.Api.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Alfavox.Interview.Infrastructure;

namespace Alfavox.Interview.Api.Services
{

    public interface ISkywalkerService
    {
        public Task<SkywalkerDetailsResponse> GetSkywalkerData();
    }

    public class SkywalkerService : ISkywalkerService
    {
        private static string ApiUrl { get; } = "https://swapi.dev/api/people/1/";

        private readonly ILoggingService _loggingService;
        private readonly IHttpClientWrapper _httpClientWrapper;


        public SkywalkerService(ILoggingService loggingService, IHttpClientWrapper httpClientWrapper)
        {
            _loggingService = loggingService;
            _httpClientWrapper = httpClientWrapper;
        }

        public async Task<SkywalkerDetailsResponse> GetSkywalkerData()
        {

            _loggingService.LogInformation($"Sending request to API: {ApiUrl}");

            HttpResponseMessage response = await _httpClientWrapper.GetAsync(ApiUrl);

            if (response.IsSuccessStatusCode)
            {
                _loggingService.LogInformation("API request successful.");

                string responseBody = await response.Content.ReadAsStringAsync();

                var lukeSkywalkerData = JsonConvert.DeserializeObject<SkywalkerDetailsResponse>(responseBody);

                var filmNames = await GetHypermediaData(lukeSkywalkerData.Films, "title");
                var vehicleNames = await GetHypermediaData(lukeSkywalkerData.Vehicles, "name");
                var starshipNames = await GetHypermediaData(lukeSkywalkerData.Starships, "name");

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

            File.WriteAllText(filePath, jsonResult);

            _loggingService.LogInformation("Reponse saved in file");
        }

        private async Task<List<string>> GetHypermediaData(List<string> urls, string propertyName)
        {
            var result = new List<string>();

                foreach (var url in urls)
                {
                    HttpResponseMessage response = await _httpClientWrapper.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        _loggingService.LogInformation("Hypermedia API request successful.");

                        string responseBody = await response.Content.ReadAsStringAsync();

                        var responseData = JsonConvert.DeserializeObject<JObject>(responseBody);

                        if (responseData.TryGetValue(propertyName, out var propertyValue) && propertyValue != null)
                        {
                            result.Add(propertyValue.ToString());
                        }
                    }

                    else
                    {
                        _loggingService.LogError($"Hypermedia API request failed with status {response.StatusCode}");

                        throw new BadRequestException($"API request failed with status {response.StatusCode}");
                    }
                }

            return result;
        }
    }
}
