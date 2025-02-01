using ManageBlockedCountriesAPI.Models;

namespace ManageBlockedCountriesAPI.Repositories
{
	public interface ICountriesRepository
	{
		Task<bool> AddCountry(string countryCode);
		Task<bool> RemoveCountry(string countryCode);
		Task<List<string>> GetBlockedCountries();
		Task<List<string>> GetBlockedCountries(int page, int pageSize, string searchTerm);
		void AddLog(LogData log);
		Task<List<LogData>> GetLogs(int page, int pageSize);
		Task<bool> AddTemporalBlock(string countryCode, TimeSpan duration);
		void RemoveTemporalBlocks();
	}
}
