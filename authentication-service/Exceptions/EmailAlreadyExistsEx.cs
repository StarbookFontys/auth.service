using System.Text.Json.Serialization;

namespace authentication_service.Exceptions
{
	[Serializable]
	public class EmailAlreadyExistsEx : Exception
	{
		public EmailAlreadyExistsEx() { }

		public EmailAlreadyExistsEx(string email)
			: base( email + " aready exists") { }
	}
}
