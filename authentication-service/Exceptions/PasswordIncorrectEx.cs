using System.Text.Json.Serialization;

namespace authentication_service.Exceptions
{
	[Serializable]
	public class PasswordIncorrectEx : Exception
	{
		public PasswordIncorrectEx() { }

		public PasswordIncorrectEx(string email)
			: base("Incorrect password for email: " + email) { }
	}
}
