using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using authentication_service.Interfaces;
using System.Collections.Generic;
using authentication_service.Exceptions;
using System.Text;

namespace authentication_service.Business
{
	public class Encryptor
	{

		public byte[] GenerateSalt()
		{
			byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

			return salt;
		}

		public Models.HashInfo Hasher(string email, string password, byte[] _salt)
		{
			string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password: password,
				salt: _salt,
				prf: KeyDerivationPrf.HMACSHA256,
				iterationCount: 1000000,
				numBytesRequested: 256 / 8));

			Models.HashInfo hashInfo = new Models.HashInfo
			{ 
				email = email,
				hashed = hashed,
				salt = _salt
			};

			return hashInfo;
		}
	}
}
