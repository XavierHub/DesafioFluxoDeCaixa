using EmpXpo.Accounting.Domain.Abstractions.Services;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;


namespace EmpXpo.Accounting.Application.Services
{
    public class ClientApiApplicationService : IClientApiApplicationService
    {
        private readonly HttpClient _client;
        private const string JSON_HEADER = "application/json";

        public ClientApiApplicationService()
        {
            _client = new HttpClient();
        }

        public async Task<T> Get<T>(string api, IDictionary<string, string>? param = null)
        {
            var queryString = "";
            if (param != null && param.Count > 0)
            {
                queryString = string.Join("/", param.Select(x => $"{x.Value}"));
            }

            var url = string.Format("{0}/{1}", api, string.Join("/", queryString));

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_HEADER));
            using (HttpResponseMessage response = await _client.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<T>() ?? Activator.CreateInstance<T>();
                }
            }
            return Activator.CreateInstance<T>();
        }

        public async Task<T> Post<T>(string api, T param)
        {
            var content = new StringContent(JsonSerializer.Serialize(param), Encoding.UTF8, JSON_HEADER);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_HEADER));
            using (HttpResponseMessage response = await _client.PostAsync(api, content))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<T>() ?? Activator.CreateInstance<T>();
                }
            }
            return Activator.CreateInstance<T>();
        }
    }
}
