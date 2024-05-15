namespace authentication_service.Interfaces
{
	public interface IRegister
	{
		public Boolean NoEmailExists(string email);
		public void SaveInfo(string email, string hashed, string salt);
		public (string salt, string hash) GetHashInformation(string email);
		public void UpdateEmail(string OldEmail, string NewEmail);
		public void UpdatePassword(string Email,  string Password, string salt);
		public Boolean IsAdmin(string email);
	}
}
