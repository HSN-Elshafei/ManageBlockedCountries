using ManageBlockedCountriesAPI.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using GeolocationApiOptions = ManageBlockedCountriesAPI.Models.GeolocationApiOptions;

namespace ManageBlockedCountriesAPI.Services
{
	public class CountriesService : ICountriesService
	{
		private readonly HttpClient _httpClient;
		private readonly string _apiKey;
		private readonly IOptions<GeolocationApiOptions> _configurationService;

		public CountriesService(HttpClient httpClient, IOptions<GeolocationApiOptions> configurationService)
		{
			_httpClient = httpClient;
			_configurationService = configurationService;
			_apiKey = _configurationService.Value.ApiKey;
		}

		public async Task<GeoData> GetGeoDataAsync(string ip)
		{
			try
			{
				//var response = await _httpClient.GetStringAsync($"&ip={ip}");
				var response = await _httpClient.GetStringAsync($"/ipgeo?apiKey={_apiKey}&ip={ip}");
				return JsonConvert.DeserializeObject<GeoData>(response);
			}
			catch (HttpRequestException ex)
			{
				return null;
			}
		}

	}
}
