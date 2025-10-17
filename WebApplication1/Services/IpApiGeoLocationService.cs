using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebApplication1.Services
{
    public class IpApiGeoLocationService : IGeoLocationService
    {
        private readonly HttpClient _httpClient;

        public IpApiGeoLocationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IpLookupResponse?> LookupIpAsync(string ip)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://ipapi.co/{ip}/json/");
                request.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; MyApp/1.0)");
                var response = await _httpClient.SendAsync(request);


                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<IpLookupResponse>(json);
                return data;
            }
            catch
            {
                return null;
            }
        }
    }

    public class IpLookupResponse
    {
        [JsonProperty("ip")]
        public string? Ip { get; set; }

        [JsonProperty("city")]
        public string? City { get; set; }

        [JsonProperty("region")]
        public string? Region { get; set; }

        [JsonProperty("country_name")]
        public string? Country { get; set; }

        [JsonProperty("country_code")]
        public string? CountryCode { get; set; }

        [JsonProperty("org")]
        public string? Org { get; set; }
    }


}
