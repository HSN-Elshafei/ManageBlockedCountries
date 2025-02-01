using ManageBlockedCountriesAPI.Models;
using ManageBlockedCountriesAPI.Repositories;
using ManageBlockedCountriesAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient<ICountriesService, CountriesService>(client =>
{
	client.BaseAddress = new Uri(builder.Configuration["GeolocationApi:BaseUrl"]);
});
builder.Services.Configure<GeolocationApiOptions>(builder.Configuration.GetSection("GeolocationApi"));

builder.Services.AddSingleton<ICountriesRepository, CountriesRepository>();
builder.Services.AddSingleton<TemporalBlockService>();
builder.Services.AddHostedService<TemporalBlockService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
