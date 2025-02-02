using ManageBlockedCountriesAPI.Models;
using ManageBlockedCountriesAPI.Repositories;
using ManageBlockedCountriesAPI.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace ManageBlockedCountriesTest
{
	public class CountriesTests
	{
		private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
		private readonly Mock<ICountriesRepository> _mockCountriesRepo;
		private readonly Mock<IOptions<GeolocationApi>> _mockOptions;
		private readonly CountriesService _countriesService;
		private readonly CountriesRepository _countriesRepository;

		public CountriesTests()
		{
			_countriesRepository = new CountriesRepository();
			_mockHttpMessageHandler = new Mock<HttpMessageHandler>();
			_mockCountriesRepo = new Mock<ICountriesRepository>();
			_mockOptions = new Mock<IOptions<GeolocationApi>>();

			_mockOptions.Setup(o => o.Value).Returns(new GeolocationApi
			{
				ApiKey = "test-key",
				BaseUrl = "https://api.ipgeolocation.io",
				CountryCodeBaseUrl = "https://restcountries.com/v3.1/alpha"
			});

			var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
			_countriesService = new CountriesService(httpClient, _mockOptions.Object, _mockCountriesRepo.Object);
		}

		[Fact]
		public async Task AddCountry_ValidCountry_ShouldAddCountry()
		{
			// Arrange
			var countryCode = "US";

			// Act
			var result = await _countriesRepository.AddCountry(countryCode);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task AddCountry_ExistingCountry_ShouldNotAddAgain()
		{
			// Arrange
			var countryCode = "US";
			await _countriesRepository.AddCountry(countryCode);

			// Act
			var result = await _countriesRepository.AddCountry(countryCode);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task RemoveCountry_ValidCountry_ShouldRemoveCountry()
		{
			// Arrange
			var countryCode = "US";
			await _countriesRepository.AddCountry(countryCode);

			// Act
			var result = await _countriesRepository.RemoveCountry(countryCode);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task RemoveCountry_NonExistentCountry_ShouldReturnFalse()
		{
			// Act
			var result = await _countriesRepository.RemoveCountry("XX");

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task GetBlockedCountries_NoFilter_ShouldReturnAllBlockedCountries()
		{
			// Arrange
			var countryCode1 = "US";
			var countryCode2 = "IN";
			await _countriesRepository.AddCountry(countryCode1);
			await _countriesRepository.AddCountry(countryCode2);

			// Act
			var result = await _countriesRepository.GetBlockedCountries();

			// Assert
			Assert.Contains(countryCode1, result);
			Assert.Contains(countryCode2, result);
		}

		[Fact]
		public async Task GetBlockedCountries_WithSearchTerm_ShouldReturnFilteredCountries()
		{
			// Arrange
			var countryCode1 = "US";
			var countryCode2 = "IN";
			await _countriesRepository.AddCountry(countryCode1);
			await _countriesRepository.AddCountry(countryCode2);

			// Act
			var result = await _countriesRepository.GetBlockedCountries(1, 10, "US");

			// Assert
			Assert.Single(result);
			Assert.Contains(countryCode1, result);
		}

		[Fact]
		public async Task AddTemporalBlock_ValidCountry_ShouldAddTemporalBlock()
		{
			// Arrange
			var countryCode = "US";
			var duration = TimeSpan.FromMinutes(10);

			// Act
			var result = await _countriesRepository.AddTemporalBlock(countryCode, duration);

			// Assert
			Assert.True(result);
		}


		[Fact]
		public void RemoveTemporalBlocks_ShouldRemoveExpiredBlocks()
		{
			// Arrange
			var countryCode = "US";
			var duration = TimeSpan.FromMilliseconds(1);
			_countriesRepository.AddTemporalBlock(countryCode, duration).Wait();

			// Act
			System.Threading.Thread.Sleep(2);
			_countriesRepository.RemoveTemporalBlocks();

			// Assert
			var result = _countriesRepository.GetBlockedCountries().Result;
			Assert.DoesNotContain(countryCode, result);
		}

		[Fact]
		public void AddLog_ShouldAddLog()
		{
			// Arrange
			var log = new LogData
			{
				IpAddress = "192.168.1.1",
				Timestamp = DateTime.UtcNow,
				CountryCode = "US",
				BlockedStatus = true,
				UserAgent = "Mozilla/5.0"
			};

			// Act
			_countriesRepository.AddLog(log);

			// Assert
			var logs = _countriesRepository.GetLogs(1, 10).Result;
			Assert.Contains(log, logs);
		}

		[Fact]
		public async Task GetLogs_ShouldReturnLogs()
		{
			// Arrange
			var log1 = new LogData
			{
				IpAddress = "192.168.1.1",
				Timestamp = DateTime.UtcNow,
				CountryCode = "US",
				BlockedStatus = true,
				UserAgent = "Mozilla/5.0"
			};
			var log2 = new LogData
			{
				IpAddress = "192.168.1.2",
				Timestamp = DateTime.UtcNow,
				CountryCode = "IN",
				BlockedStatus = false,
				UserAgent = "Mozilla/5.0"
			};

			_countriesRepository.AddLog(log1);
			_countriesRepository.AddLog(log2);

			// Act
			var logs = await _countriesRepository.GetLogs(1, 10);

			// Assert
			Assert.Equal(2, logs.Count);
			Assert.Contains(log1, logs);
			Assert.Contains(log2, logs);
		}


		[Fact]
		public async Task IsBlockedAsync_BlockedCountry_ShouldReturnTrue()
		{
			_mockCountriesRepo.Setup(repo => repo.GetBlockedCountries())
				.ReturnsAsync(new List<string> { "US", "GB" });

			var result = await _countriesService.IsBlockedAsync("US");

			Assert.True(result);
		}

		[Fact]
		public async Task IsBlockedAsync_NotBlockedCountry_ShouldReturnFalse()
		{
			_mockCountriesRepo.Setup(repo => repo.GetBlockedCountries())
				.ReturnsAsync(new List<string> { "US", "GB" });

			var result = await _countriesService.IsBlockedAsync("EG");

			Assert.False(result);
		}
		//[Fact]
		//public async Task GetGeoDataAsync_ValidIp_ShouldReturnGeoData()
		//{
		//	// Arrange
		//	var expectedGeoData = new GeoData { Ip = "8.8.8.8", Country_Code = "US" };

		//	var response = new HttpResponseMessage(HttpStatusCode.OK)
		//	{
		//		Content = new StringContent("{\"Ip\":\"8.8.8.8\",\"Country_Code\":\"US\"}")
		//	};

		//	_mockHttpMessageHandler
		//		.Protected()
		//		.Setup<Task<HttpResponseMessage>>(
		//			"SendAsync",
		//			ItExpr.IsAny<HttpRequestMessage>(),
		//			ItExpr.IsAny<CancellationToken>()
		//		)
		//		.ReturnsAsync(response);

		//	// Act
		//	var result = await _countriesService.GetGeoDataAsync("8.8.8.8");

		//	// Assert
		//	Assert.NotNull(result);
		//	Assert.Equal(expectedGeoData.Ip, result.Ip);
		//	Assert.Equal(expectedGeoData.Country_Code, result.Country_Code);
		//}
	}
}
