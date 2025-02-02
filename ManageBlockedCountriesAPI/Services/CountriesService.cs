using ManageBlockedCountriesAPI.Models;
using ManageBlockedCountriesAPI.Repositories;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ManageBlockedCountriesAPI.Services
{
	public class CountriesService : ICountriesService
	{
		private readonly HttpClient _httpClient;
		private readonly string _apiKey;
		private readonly string _countryCodeBaseUrl;
		private readonly IOptions<GeolocationApi> _configurationService;
		private readonly ICountriesRepository _countriesRepo;

		public CountriesService(HttpClient httpClient, IOptions<GeolocationApi> configurationService, ICountriesRepository countriesRepo)
		{
			_httpClient = httpClient;
			_configurationService = configurationService;
			_apiKey = _configurationService.Value.ApiKey;
			_countryCodeBaseUrl = _configurationService.Value.CountryCodeBaseUrl;
			_countriesRepo = countriesRepo;
		}

		public async Task<GeoData> GetGeoDataAsync(string ip)
		{
			try
			{
				var response = await _httpClient.GetStringAsync($"/ipgeo?apiKey={_apiKey}&ip={ip}");
				return JsonConvert.DeserializeObject<GeoData>(response);
			}
			catch (HttpRequestException ex)
			{
				return null;
			}
		}

		public async Task<GeoData> GetCurrentGeoDataAsync()
		{
			try
			{
				var response = await _httpClient.GetStringAsync($"/ipgeo?apiKey={_apiKey}");
				return JsonConvert.DeserializeObject<GeoData>(response);
			}
			catch (HttpRequestException ex)
			{
				return null;
			}
		}

		public async Task<bool> IsValidCountryCodeAsync(string countryCode)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(countryCode))
					return false;

				var response = await _httpClient.GetAsync($"{_countryCodeBaseUrl}/{countryCode}?fields=cca2");

				return response.IsSuccessStatusCode;
			}
			catch (HttpRequestException ex)
			{
				throw;
			}
		}

		public async Task<bool> IsBlockedAsync(string countryCode)
		{
			var country = await _countriesRepo.GetBlockedCountries();
			var isBlocked = country.Contains(countryCode);
			return isBlocked;
		}
	}
}