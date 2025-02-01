using ManageBlockedCountriesAPI.Models;

namespace ManageBlockedCountriesAPI.Services
{
	public interface ICountriesService
	{
		Task<GeoData> GetGeoDataAsync(string ip);
		Task<GeoData> GetCurrentGeoDataAsync();
		Task<bool> IsValidCountryCodeAsync(string countryCode);
		Task<bool> IsBlockedAsync(string countryCode);
	}
}
