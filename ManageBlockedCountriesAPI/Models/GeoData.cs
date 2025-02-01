namespace ManageBlockedCountriesAPI.Models
{
	using Newtonsoft.Json;

	public class GeoData
	{
		[JsonProperty("ip")]
		public string Ip { get; set; }
		[JsonProperty("Country_name")]
		public string Country_Name { get; set; }

		[JsonProperty("country_code2")]
		public string Country_Code { get; set; }

		[JsonProperty("isp")]
		public string Isp { get; set; }
	}
}
