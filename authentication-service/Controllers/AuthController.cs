using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using authentication_service.Business;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace authentication_service.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly string ConnectionString;

		public AuthController(IConfiguration configuration)
		{
			_configuration = configuration;
			ConnectionString = _configuration.GetSection("ConnectionStrings").GetValue<string>("DevDatabase");
		}
		// GET: api/<AuthController>

		// GET api/<AuthController>/5
		[HttpGet("/P/{password}")]
		public string Get(string empty, string password)
		{
			Encryptor encryptor = new Encryptor();
			//return encryptor.Hasher(password);
			return "n/a";
		}

		[HttpGet("{salt}")]
		public string Get(string salt)
		{
			Encryptor encryptor = new Encryptor();
			//return encryptor.Hasher(Encoding.ASCII.GetBytes(salt), "SAMPLE");
			return ConnectionString;
		}

		[HttpGet()]
		public byte[] Get()
		{
			Encryptor encryptor = new Encryptor();
			return encryptor.salt();
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
