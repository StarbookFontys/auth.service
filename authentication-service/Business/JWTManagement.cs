using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace authentication_service.Business
{
	public class JWTManagement
	{
		private readonly string _secretKey;
		private readonly string _issuer;

		public JWTManagement(string secretKey, string issuer)
		{
			_secretKey = secretKey; //UserSecret 
			_issuer = issuer; //Website name 
		}

		public string GenerateToken(string email, string userLevel)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_secretKey);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
				new Claim(ClaimTypes.Email, email),
				new Claim("user_level", userLevel)
				}),
				Expires = DateTime.UtcNow.AddDays(7),
				Issuer = _issuer,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
