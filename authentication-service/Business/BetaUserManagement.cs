using authentication_service.DAL;
using authentication_service.Interfaces;
using authentication_service.RabbitMq;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace authentication_service.Business
{
	public class BetaUserManagement
	{
		private readonly IRegister register;
		private readonly IJWTManagement JWT;

		public BetaUserManagement(IRegister _register, IJWTManagement jWTManagement)
		{
			register = _register;
			JWT = jWTManagement;
		}

		public void CreateBetaUsersBatch(string token, double targetPercentage)
		{
			try
			{
				string JWTemail = JWT.JWTDecoder(token).email;
				Boolean JWTIsAdmin = JWT.JWTDecoder(token).isAdmin;
				if (JWTIsAdmin)
				{
					register.CreateBetaUsersBatch(targetPercentage);
				}
				else
				{
					throw new UnauthorizedAccessException();
				}
			}
			catch (SecurityTokenValidationException ex)
			{
				throw ex;
			}
		}
	}
}
