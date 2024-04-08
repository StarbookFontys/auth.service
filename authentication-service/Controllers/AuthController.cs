using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using authentication_service.Business;
using System.Text;
using authentication_service.DAL;
using Npgsql;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace authentication_service.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{

		private readonly IConfiguration _configuration;
		private readonly string ConnectionString;
		private readonly Encryptor encryptor;

		public AuthController(IConfiguration configuration)
		{
			_configuration = configuration;
			ConnectionString = _configuration.GetSection("ConnectionStrings").GetValue<string>("DevDatabase");
			var con = new Connection(ConnectionString);
			var _register = new Register(con);
			encryptor = new Encryptor(_register);
		}
		[HttpGet("/P/{password}")]
		public string Get(string empty, string password)
		{
			return "n/a";
		}

		[HttpPost("{email}/{password}")]
		public void Post(string email, string password)
		{
			encryptor.SaveInfo(email, password);
		}

		// POST api/<AuthController>
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/<AuthController>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/<AuthController>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
