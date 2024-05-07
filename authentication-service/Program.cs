using AspNetCoreRateLimit;
using Microsoft.AspNetCore.RateLimiting;
using Prometheus;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

var provider = builder.Services.BuildServiceProvider();

var _configuration = provider.GetRequiredService<IConfiguration>();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
var url = $"http://0.0.0.0:{port}";
var target = Environment.GetEnvironmentVariable("TARGET") ?? "World";

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRateLimiter(options =>
{
	options.RejectionStatusCode = 429;
	options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
		RateLimitPartition.GetFixedWindowLimiter(
			partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
			factory: partition => new FixedWindowRateLimiterOptions
			{
				AutoReplenishment = true,
				PermitLimit = 10,
				QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
				QueueLimit = 0,
				Window = TimeSpan.FromSeconds(5)
			}));
});

var app = builder.Build();

app.UseMetricServer();

app.UseRateLimiter();

app.MapGet("/", () => "The authentication api. Hello {target}!");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run(url);
