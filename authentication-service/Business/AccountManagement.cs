using authentication_service.DAL;
using authentication_service.Exceptions;
using authentication_service.Interfaces;
using authentication_service.RabbitMq;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

namespace authentication_service.Business
{
	public class AccountManagement
	{
		private readonly IUnregister unregister;
		private readonly IRegister register;
		private readonly Encryptor encryptor;
		private readonly IRabbitMqManagement rabbitMqManagement;

		public AccountManagement(IUnregister _unregister, IRegister _register, IRabbitMqManagement _rabbitMqManagement)
		{
			unregister = _unregister;
			register = _register;
			encryptor = new Encryptor();
			rabbitMqManagement = _rabbitMqManagement;
		}

		public void SaveInformation(string email, string password)
		{
			var NoEmailExists = register.NoEmailExists(email);
			if (NoEmailExists == true)
			{
				throw new EmailAlreadyExistsEx(email);
			}
			else
			{
				Models.HashInfo hashinfo = encryptor.Hasher(email, password, encryptor.GenerateSalt());
				register.SaveInfo(hashinfo.email, hashinfo.hashed, Convert.ToBase64String(hashinfo.salt));
			}
		}

		public Boolean VerifyInformation(string email, string password)
		{
			byte[] salt = Convert.FromBase64String(register.GetHashInformation(email).salt);
			string StoredHash = register.GetHashInformation(email).hash;
			Models.HashInfo ReceivedHash = encryptor.Hasher(email, password, salt);
			if (StoredHash == ReceivedHash.hashed)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public void UpdateEmail(string OldEmail, string NewEmail)
		{
			register.UpdateEmail(OldEmail, NewEmail);
		}

		public void UpdatePassword(string Email,  string Password)
		{
			var NewPasswordInformation = encryptor.Hasher(Email, Password, encryptor.GenerateSalt());

			register.UpdatePassword(NewPasswordInformation.email, NewPasswordInformation.hashed, Convert.ToBase64String(NewPasswordInformation.salt));
		}

		public void DeleteInformation(string email, string password)
		{
			if (VerifyInformation(email, password))
			{
				unregister.RemoveUserData(email);
				var BrokerMessage = new RabbitMqMessageModel
				{
					Action = "Delete",
					Value = email
				};
				rabbitMqManagement.SendMessage(BrokerMessage);
			}
			else
			{
				throw new PasswordIncorrectEx(email);
			}
		}
	}
}
