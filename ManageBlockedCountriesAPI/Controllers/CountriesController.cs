using ManageBlockedCountriesAPI.Models;
using ManageBlockedCountriesAPI.Repositories;
using ManageBlockedCountriesAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManageBlockedCountriesAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CountriesController : ControllerBase
	{
		private readonly ICountriesRepository _countriesRepo;
		private readonly ICountriesService _countriesService;

		public CountriesController(ICountriesRepository countriesRepo, ICountriesService countriesService)
		{
			_countriesRepo = countriesRepo;
			_countriesService = countriesService;
		}

		[HttpPost("block")]
		public IActionResult BlockCountry([FromBody] string countryCode)
		{
			if (string.IsNullOrEmpty(countryCode))
				return BadRequest("Country code is required.");

			if (_countriesRepo.AddCountry(countryCode))
				return Ok($"Country {countryCode} blocked successfully.");

			return Conflict($"Country {countryCode} is already blocked.");
		}

		[HttpDelete("block/{countryCode}")]
		public IActionResult UnblockCountry(string countryCode)
		{
			if (_countriesRepo.RemoveCountry(countryCode))
				return Ok($"Country {countryCode} unblocked successfully.");

			return NotFound($"Country {countryCode} is not blocked.");
		}

		[HttpGet("blocked")]
		public IActionResult GetBlockedCountries([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchTerm = null)
		{
			return Ok(_countriesRepo.GetBlockedCountries(page, pageSize, searchTerm));
		}

		[HttpGet("ip/lookup")]
		public async Task<IActionResult> Lookup([FromQuery] string ipAddress)
		{
			if (string.IsNullOrEmpty(ipAddress))
			{
				ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
			}

			// Validate IP address
			if (!System.Net.IPAddress.TryParse(ipAddress, out var parsedIpAddress))
			{
				return BadRequest("Invalid IP address format.");
			}

			var geoData = await _countriesService.GetGeoDataAsync(ipAddress);
			if (geoData == null)
			{
				return StatusCode(500, "Failed to fetch geo data from the external API.");
			}

			return Ok(geoData);
		}

		[HttpGet("ip/check-block")]
		public async Task<IActionResult> CheckBlock()
		{
			var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
			if (ipAddress == "::1" || ipAddress == "127.0.0.1")
			{
				ipAddress = "154.178.198.193";
			}


			if (!System.Net.IPAddress.TryParse(ipAddress, out var parsedIpAddress))
			{
				return BadRequest("Invalid IP address format.");
			}

			var geoData = await _countriesService.GetGeoDataAsync(ipAddress);


			if (geoData == null)
			{
				return StatusCode(500, "Failed to fetch geo data from the external API.");
			}


			if (string.IsNullOrEmpty(geoData.Country_Code))
			{
				return StatusCode(500, "Country code not found in the API response.");
			}

			var isBlocked = _countriesRepo.GetBlockedCountries().Contains(geoData.Country_Code);

			_countriesRepo.AddLog(new LogEntry
			{
				IpAddress = ipAddress,
				Timestamp = DateTime.UtcNow,
				CountryCode = geoData.Country_Code,
				BlockedStatus = isBlocked,
				UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
			});

			return Ok(new
			{
				IpAddress = ipAddress,
				CountryCode = geoData.Country_Code,
				IsBlocked = isBlocked
			});
		}

		[HttpGet("logs/blocked-attempts")]
		public IActionResult GetBlockedAttempts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			return Ok(_countriesRepo.GetLogs(page, pageSize));
		}
		[HttpPost("temporal-block")]
		public IActionResult TemporalBlockCountry([FromBody] TemporalBlockRequest request)
		{
			if (request.DurationMinutes < 1 || request.DurationMinutes > 1440)
				return BadRequest("Duration must be between 1 and 1440 minutes.");

			if (!_countriesRepo.AddTemporalBlock(request.CountryCode, TimeSpan.FromMinutes(request.DurationMinutes)))
				return Conflict($"Country {request.CountryCode} is already temporarily blocked.");

			return Ok($"Country {request.CountryCode} temporarily blocked for {request.DurationMinutes} minutes.");
		}
	}
}
