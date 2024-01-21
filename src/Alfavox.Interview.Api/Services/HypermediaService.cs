using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Alfavox.Interview.Api.Exceptions;

namespace Alfavox.Interview.Api.Services
{
    public interface IHypermediaService
    {
        public Task<List<string>> GetHypermediaData(List<string> urls, string propertyName);
    }

    public class HypermediaService : IHypermediaService
    {
        private readonly IHttpContextWrapper _httpContextWrapper;

        public HypermediaService(IHttpContextWrapper httpContextWrapper)
        {
            _httpContextWrapper = httpContextWrapper;
        }
        public async Task<List<string>> GetHypermediaData(List<string> urls, string propertyName)
        {
            var result = new List<string>();

            foreach (var url in urls)
            {
                HttpResponseMessage response = await _httpContextWrapper.GetAsync(url.ToString());

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    var responseData = JsonConvert.DeserializeObject<JObject>(responseBody);

                    if (responseData.TryGetValue(propertyName, out var propertyValue) && propertyValue != null)
                    {
                        result.Add(propertyValue.ToString());
                    }
                }

                else
                {
                    throw new BadRequestException($"API request failed with status {response.StatusCode}");
                }
            }

            return result;
        }
    }
}
