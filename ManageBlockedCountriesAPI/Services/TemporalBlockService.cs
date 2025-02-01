using ManageBlockedCountriesAPI.Repositories;

namespace ManageBlockedCountriesAPI.Services
{
	public class TemporalBlockService : BackgroundService
	{
		private readonly ICountriesRepository _countriesRepository;

		public TemporalBlockService(ICountriesRepository countriesRepository)
		{
			_countriesRepository = countriesRepository;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				_countriesRepository.RemoveTemporalBlocks();
				await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
			}
		}
	}
}
