using authentication_service.Business;
using authentication_service.DAL;
using authentication_service.RabbitMq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace authentication_service.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AdminController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly string ConnectionString;
		private readonly AccountManagement accountManagement;
		private readonly RabbitMqManagement rabbitMqManagement;
		private readonly JWTManagement JWTManager;
		private readonly BetaUserManagement betaUserManagement;

		public AdminController(IConfiguration configuration)
		{
			_configuration = configuration;
			ConnectionString = _configuration["Database:ConnectionString"];
			var con = new Connection(ConnectionString);
			var _register = new Register(con);
			var _unregister = new Unregister(con);
			rabbitMqManagement = new RabbitMqManagement();
			JWTManager = new JWTManagement(_configuration.GetValue<string>("ApplicationSettings:JWT_Secret"), _configuration["JWT:Issuer"]);
			accountManagement = new AccountManagement(_unregister, _register, rabbitMqManagement, JWTManager);
			betaUserManagement = new BetaUserManagement(_register, JWTManager);
		}
		[HttpPost("{targetPercentage}/{Token}")]
		public IResult Post(double targetPercentage, string Token)
		{
			try
			{
				betaUserManagement.CreateBetaUsersBatch(Token, targetPercentage);
				return Results.Ok();
			}
			catch (UnauthorizedAccessException ex)
			{
				return Results.Unauthorized();
			}
			catch (SecurityTokenValidationException ex)
			{
				return Results.Problem("Error during formatting processs. Token formatting is incorrect.");
			}
		}

		[HttpDelete("{Email}/{Token}")]
		public IResult Delete(string Email, string Token)
		{
			try
			{
				accountManagement.AdminDelete(Email, Token);
				return Results.Ok();
			}
			catch (UnauthorizedAccessException ex)
			{
				return Results.Unauthorized();
			}
			catch(SecurityTokenValidationException ex)
			{
				return Results.Problem("Error during formatting processs. Token formatting is incorrect.");
			}
		}
	}
}
