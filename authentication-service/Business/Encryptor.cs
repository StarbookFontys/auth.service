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

		private Models.HashInfo Hasher(string email, string password, byte[] _salt)
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

		public void SaveInfo(string email, string password)
		{
			var NoEmailExists = register.NoEmailExists(email);
			if (NoEmailExists == true)
			{
				throw new EmailAlreadyExistsEx(email);
			}
			else
			{
				Models.HashInfo hashinfo = Hasher(email, password, GenerateSalt());
				register.SaveInfo(hashinfo.email, hashinfo.hashed, Convert.ToBase64String(hashinfo.salt));
			}
		}

		public Boolean VerifyInformation(string email, string password)
		{
			byte[] salt = Convert.FromBase64String(register.GetHashInformation(email).salt);
			string StoredHash = register.GetHashInformation(email).hash;
			Models.HashInfo ReceivedHash = Hasher(email, password, salt);
			if(StoredHash == ReceivedHash.hashed)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
