using ManageBlockedCountriesAPI.Models;
using System.Collections.Concurrent;
namespace ManageBlockedCountriesAPI.Repositories
{
	public class CountriesRepository : ICountriesRepository
	{
		private readonly ConcurrentDictionary<string, BlockInfo> _countryBlocks = new();
		private readonly ConcurrentQueue<LogData> _logs = new();


		public async Task<bool> AddCountry(string countryCode)
		{
			return _countryBlocks.TryAdd(countryCode, new BlockInfo { IsTemporary = false });
		}

		public async Task<bool> RemoveCountry(string countryCode)
		{
			return _countryBlocks.TryRemove(countryCode, out _);
		}

		public async Task<List<string>> GetBlockedCountries(int page, int pageSize, string searchTerm)
		{
			var query = _countryBlocks.Where(x => !x.Value.IsTemporary || x.Value.ExpirationTime > DateTime.UtcNow)
									   .Select(x => x.Key).AsQueryable();

			if (!string.IsNullOrEmpty(searchTerm))
			{
				query = query.Where(code => code.ToUpper().Contains(searchTerm.ToUpper()));
			}
			return query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
		}

		public async Task<List<string>> GetBlockedCountries()
		{
			return _countryBlocks.Where(x => !x.Value.IsTemporary || x.Value.ExpirationTime > DateTime.UtcNow)
								  .Select(x => x.Key).ToList();
		}

		public void AddLog(LogData log)
		{
			_logs.Enqueue(log);
		}

		public async Task<List<LogData>> GetLogs(int page, int pageSize)
		{
			return _logs.Skip((page - 1) * pageSize).Take(pageSize).ToList();
		}

		public async Task<bool> AddTemporalBlock(string countryCode, TimeSpan duration)
		{
			var expirationTime = DateTime.UtcNow.Add(duration);
			return _countryBlocks.TryAdd(countryCode, new BlockInfo { IsTemporary = true, ExpirationTime = expirationTime });
		}

		public void RemoveTemporalBlocks()
		{
			var now = DateTime.UtcNow;
			foreach (var country in _countryBlocks.Where(x => x.Value.IsTemporary && x.Value.ExpirationTime <= now).Select(x => x.Key).ToList())
			{
				_countryBlocks.TryRemove(country, out _);
			}
		}

		private class BlockInfo
		{
			public bool IsTemporary { get; set; }
			public DateTime ExpirationTime { get; set; }
		}
	}

}
