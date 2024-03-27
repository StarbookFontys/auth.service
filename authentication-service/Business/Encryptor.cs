using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace authentication_service.Business
{
	public class Encryptor
	{
		public byte[] salt()
		{
			byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

			return salt;
		}

		public void Hasher(string password)
		{
			 byte []_salt = salt();
			string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password: password,
				salt: _salt,
				prf: KeyDerivationPrf.HMACSHA256,
				iterationCount: 1000000,
				numBytesRequested: 256 / 8));
		}
	}
}
