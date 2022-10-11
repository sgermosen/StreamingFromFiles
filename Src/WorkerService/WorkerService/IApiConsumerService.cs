using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WorkerService.Models;

namespace WorkerService
{
    public interface IApiConsumerService
    {
        Task<Book> GetFromApi(string item, string fileName);
        void Run(string[] items, string fileName);

    }

    public class ApiConsumerService : IApiConsumerService
    {
        private readonly ILogger<ApiConsumerService> _logger;
        private readonly string _apiEndpoint;

        public ApiConsumerService(ILogger<ApiConsumerService> logger, IOptions<AppSettings> settings)
        {
            _logger = logger;
            _apiEndpoint = settings.Value.ApiEndpoint;
        }
        public async Task<Book> GetFromApi(string item, string fileName)
        {
            _logger.LogInformation("Start call the API");
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{_apiEndpoint}/{item}.json");
                if (response.IsSuccessStatusCode && response.Content.Headers.ContentLength > 0)
                {
                    var element = JsonConvert.DeserializeObject<Book>(await response.Content.ReadAsStringAsync());
                    element.DataRetrievalType = DataRetrievalType.Server;
                    element.Isbn = item;
                    return element;
                }
                else
                    _logger.LogError($"Problem retreaving data from: {item}");
            }
            _logger.LogInformation("End call the API");
            return null;
        }
        public async void Run(string[] items, string fileName)
        {
            _logger.LogInformation("Start call the API");
            var books = new List<Book>();
            using (var httpClient = new HttpClient())
            {
                foreach (var item in items)
                {
                    var response = await httpClient.GetAsync($"{_apiEndpoint}/{item}.json");
                    if (response.IsSuccessStatusCode && response.Content.Headers.ContentLength > 0)
                    {
                        // httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
                        // httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var element = JsonConvert.DeserializeObject<Book>(await response.Content.ReadAsStringAsync());
                        element.DataRetrievalType = DataRetrievalType.Server;
                        element.Isbn = item;
                        books.Add(element);
                    }
                    else
                        _logger.LogError($"Problem retreaving data from: {item}");
                }
            }
            if (books.Any())
                BookContext.Instance.Books.AddRange(books);
            _logger.LogInformation("End call the API");
        }
    }
}
