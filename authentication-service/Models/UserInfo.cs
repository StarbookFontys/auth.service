using authentication_service.Enums;

namespace authentication_service.Models
{
	public class UserInfo
	{
		public string email { get; set; }
		public Userlevel AccessLevel {get; set; }
	}
}
