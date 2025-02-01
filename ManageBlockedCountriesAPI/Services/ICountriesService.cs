using ManageBlockedCountriesAPI.Models;

namespace ManageBlockedCountriesAPI.Services
{
	public interface ICountriesService
	{
		Task<GeoData> GetGeoDataAsync(string ip);
	}
}
