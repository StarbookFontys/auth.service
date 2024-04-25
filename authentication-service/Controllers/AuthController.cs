using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using authentication_service.Business;
using System.Text;
using authentication_service.DAL;
using Npgsql;
using authentication_service.Exceptions;
using authentication_service.RabbitMq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace authentication_service.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{

		private readonly IConfiguration _configuration;
		private readonly string ConnectionString;
		private readonly Encryptor encryptor;
		private readonly AccountManagement accountManagement;
		private readonly RabbitMqManagement rabbitMqManagement;

		public AuthController(IConfiguration configuration)
		{
			_configuration = configuration;
			ConnectionString = _configuration.GetSection("ConnectionStrings").GetValue<string>("DevDatabase");
			var con = new Connection(ConnectionString);
			var _register = new Register(con);
			var _unregister = new Unregister(con);
			rabbitMqManagement = new RabbitMqManagement();
			accountManagement = new AccountManagement(_unregister, _register, rabbitMqManagement);
		}
		[HttpGet("/VerifyPassword/{email}/{password}")]
		public Boolean Get(string email, string password)
		{
			return accountManagement.VerifyInformation(email, password);
		}

		[HttpPost("/CreateAccount/{email}/{password}")]
		public IResult Post(string email, string password)
		{
			try
			{
				accountManagement.SaveInformation(email, password);
				return Results.Ok();
			}
			catch(PasswordIncorrectEx ex)
			{
				return Results.Problem(ex.Message);
			}
		}

		// POST api/<AuthController>
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/<AuthController>/5
		[HttpPut("/UpdateEmail/{OldEmail}/{NewEmail}")]
		public void Put(string OldEmail, string NewEmail)
		{
			accountManagement.UpdateEmail(OldEmail, NewEmail);
		}

		[HttpPut("/UpdatePassword/{Email}/{password}")]
		public void UpdatePassword(string Email, string password)
		{
			accountManagement.UpdatePassword(Email, password);
		}

		// DELETE api/<AuthController>/5
		[HttpDelete("/DeleteAccount/{email}/{password}")]
		public IResult Delete(string email, string password)
		{
			try
			{
				accountManagement.DeleteInformation(email, password);
				return Results.Ok();
			}
			catch(PasswordIncorrectEx ex)
			{
				return Results.Problem(ex.Message);
			}
		}
	}
}
