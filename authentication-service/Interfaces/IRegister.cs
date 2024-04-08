namespace authentication_service.Interfaces
{
	public interface IRegister
	{
		public Boolean NoEmailExists(string email);
		public void SaveInfo(string email, string hashed, byte[] salt);
		public string GetHash(string email);
	}
}
