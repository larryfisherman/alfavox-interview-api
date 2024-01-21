namespace Alfavox.Interview.Api.Services
{
    public interface IHttpContextWrapper
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);
    }

    public class HttpContextWrapper : IHttpContextWrapper
    {
        private readonly HttpClient _httpClient;

        public HttpContextWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return _httpClient.GetAsync(requestUri);
        }
    }
}