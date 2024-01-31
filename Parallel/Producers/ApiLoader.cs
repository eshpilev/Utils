using System.Net;
using Newtonsoft.Json;
using ParallelPages.Models;

namespace ParallelPages.Producers
{
    public class ApiLoader : IProducer<StateInfo>, IDisposable
    {
        private readonly HttpClient _httpClient;
        public string Url { get; set; } = "";

        public ApiLoader(HttpClient httpClient = null)
        {
            if (httpClient is null)
                _httpClient = CreateHttpClient();
        }

        public IEnumerable<StateInfo> GetPageData(int limit, int offset)
        {
            try
            {
                var response = _httpClient.GetAsync($"{Url}/items?limit={limit}&offset={offset}").Result;
                if (response.StatusCode != HttpStatusCode.OK)
                    return null;
                var responseBody = response.Content.ReadAsStringAsync().Result;
                return ParseResponse(responseBody);
            }
            catch (Exception ex)
            {
                throw new Exception($"Message: {ex.Message}", ex);
            }
        }
        private static IEnumerable<StateInfo> ParseResponse(string responseBody)
        {
            var actualDateTime = DateTime.Now;
            var threadNum = Thread.CurrentThread.ManagedThreadId;
            var items = JsonConvert.DeserializeObject<IEnumerable<StateInfo>>(responseBody)?.ToArray();
            if (items == null)
                return null;
            foreach (var item in items)
            {
                item.ActualDateTime = actualDateTime;
                item.ThreadNum = threadNum;
            }
            return items;
        }

        private HttpClient CreateHttpClient()
        {
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            return new HttpClient(httpClientHandler);
        }
        public void Dispose() => _httpClient?.Dispose();
    }
}
