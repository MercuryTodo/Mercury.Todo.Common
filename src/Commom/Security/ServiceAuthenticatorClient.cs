using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.Security
{
    public class ServiceAuthenticatorClient : IServiceAuthenticatorClient
    {
        private readonly string AuthenticationEndpoint = "/authenticate";
        private readonly HttpClient _httpClient;

        public ServiceAuthenticatorClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Remove("Accept");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<JwtBasic> AuthenticateAsync(string serviceUrl, Credentials credentials)
        {
            var data = new
            {
                username = credentials.Username,
                password = credentials.Password
            };

            serviceUrl = serviceUrl.EndsWith("/") ?
                         serviceUrl.Remove(serviceUrl.Length - 1) :
                         serviceUrl;

            var response = await PostAsync($"{serviceUrl}{AuthenticationEndpoint}", data);
            var token = await DeserializeAsync<JwtBasic>(response);
            if (token == null)
            {
                return null;
            }

            return token;
        }

        private async Task<HttpResponseMessage> PostAsync(string endpoint, object data)
        {
            var payload = JsonConvert.SerializeObject(data);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            return await _httpClient.PostAsync(endpoint, content);
        }

        private static async Task<T> DeserializeAsync<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                return default(T);

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}