using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using authentication_service.Interfaces;
using System.Collections.Generic;

namespace authentication_service.Business
{
	public class Encryptor
	{
		private readonly IRegister register;

		public Encryptor(IRegister _register) 
		{
			register = _register;
		}

		private byte[] GenerateSalt()
		{
			byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

			return salt;
		}

		private Models.HashInfo Hasher(string email, string password)
		{
			 byte []_salt = GenerateSalt();
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

		public void SaveInfo(string email, string password)
		{
			var NoEmailExists = register.NoEmailExists(email);
			if (NoEmailExists == true)
			{
				throw new Exception();
			}
			else
			{
				Models.HashInfo hashinfo = Hasher(email, password);
				register.SaveInfo(hashinfo.email, hashinfo.hashed, hashinfo.salt);
			}
		}
	}
}
