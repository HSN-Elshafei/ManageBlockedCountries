using ManageBlockedCountriesAPI.Models;

namespace ManageBlockedCountriesAPI.Repositories
{
	public interface ICountriesRepository
	{
		bool AddCountry(string countryCode);
		bool RemoveCountry(string countryCode);
		List<string> GetBlockedCountries();
		List<string> GetBlockedCountries(int page, int pageSize, string searchTerm);
		void AddLog(LogEntry log);
		List<LogEntry> GetLogs(int page, int pageSize);
		bool AddTemporalBlock(string countryCode, TimeSpan duration);
		void RemoveTemporalBlocks();
	}
}
