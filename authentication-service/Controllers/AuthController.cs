using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using authentication_service.Business;
using System.Text;
using authentication_service.DAL;
using Npgsql;
using authentication_service.Exceptions;
using authentication_service.RabbitMq;
using authentication_service.Models;
using authentication_service.Enums;
using authentication_service.GCloud;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace authentication_service.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{

		private readonly IConfiguration _configuration;
		private readonly string ConnectionString;
		private readonly AccountManagement accountManagement;
		private readonly RabbitMqManagement rabbitMqManagement;
		private readonly JWTManagement JWTManager;
		private readonly JWTManagement JWT;
		private readonly GCSecretManager gCSecretManager;

		public AuthController(IConfiguration configuration)
		{
			_configuration = configuration;
			ConnectionString = _configuration["Database:ConnectionString"];
			var con = new Connection(ConnectionString);	
			gCSecretManager = new GCSecretManager();
			//ConnectionString = gCSecretManager.GetSecret("decisive-mapper-422519-b8", "AuthDatabaseConString");
			//var con = new Connection(ConnectionString);
			var _register = new Register(con);
			var _unregister = new Unregister(con);
			rabbitMqManagement = new RabbitMqManagement();
			JWTManager = new JWTManagement(_configuration.GetValue<string>("ApplicationSettings:JWT_Secret"), _configuration["JWT:Issuer"]);
			accountManagement = new AccountManagement(_unregister, _register, rabbitMqManagement, JWTManager);
			JWT = new JWTManagement(_configuration["JWT:Key"], _configuration["JWT:Issuer"]);

		}
		[HttpGet("/VerifyPassword/{email}/{password}")]
		public JWTReturn Get(string email, string password)
		{
			return accountManagement.CreateJWT(email, password);
		}

		[HttpGet("/VerifyJWt/{JWT}")]
		public IResult Get(string JWT)
		{
			try
			{
				JWTManager.ValidateToken(JWT);
				return Results.Ok();
			}
			catch (Exception ex)
			{
				return Results.Problem("Error during JWT validation. Incorrect JWT formatting");
			}
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
