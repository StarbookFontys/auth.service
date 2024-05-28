using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using authentication_service.Interfaces;
using System.Security.Principal;
using System.IO;

namespace authentication_service.Business
{
	public class JWTManagement : IJWTManagement
	{
		private readonly string _secretKey;
		private readonly string _issuer;

		public JWTManagement(string secretKey, string issuer)
		{
			_secretKey = secretKey; //UserSecret 
			_issuer = issuer; //Website name
		}

		public string GenerateJWTToken(string email, string userlevel, bool betaUser)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Email, email),
				new Claim(ClaimTypes.Role, userlevel),
				new Claim("betaUser", betaUser.ToString())
			};
			var jwtToken = new JwtSecurityToken(
				claims: claims,
				notBefore: DateTime.UtcNow,
				expires: DateTime.UtcNow.AddDays(30),
				signingCredentials: new SigningCredentials(
					new SymmetricSecurityKey(
					   Encoding.UTF8.GetBytes(_secretKey)
						),
					SecurityAlgorithms.HmacSha256Signature)
				);
			return new JwtSecurityTokenHandler().WriteToken(jwtToken);
		}

		public bool ValidateToken(string AuthToken)
		{
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var validationParameters = new TokenValidationParameters()
				{
					ValidateLifetime = false,
					ValidateIssuer = false,
					ValidateAudience = false,
					IssuerSigningKey = new SymmetricSecurityKey(
						   Encoding.UTF8.GetBytes(_secretKey)
							)
				};
				SecurityToken validatedToken;
			IPrincipal principal = tokenHandler.ValidateToken(AuthToken, validationParameters, out validatedToken);
				return true;
			}
			catch (Exception ex)
			{
				throw new FormatException();
			}
		}

		public (string email, Boolean isAdmin) JWTDecoder(string jwtToken)
		{
			try
			{
				var handler = new JwtSecurityTokenHandler();
				var jsonToken = handler.ReadToken(jwtToken);
				var DecodedToken = jsonToken as JwtSecurityToken;

				var emailClaim = DecodedToken.Claims.FirstOrDefault(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
				var roleClaim = DecodedToken.Claims.FirstOrDefault(claim => claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

				if (emailClaim == null || roleClaim == null)
				{
					throw new SecurityTokenValidationException();
				}

				bool roleValue = bool.Parse(roleClaim);
				return (emailClaim, roleValue);
			}
			catch(Exception ex)
			{
				throw new SecurityTokenValidationException();
			}
		}
	}
}
