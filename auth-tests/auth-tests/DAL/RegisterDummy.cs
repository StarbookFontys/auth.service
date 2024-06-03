using authentication_service.Interfaces;
namespace auth_tests.DAL
{
	internal class RegisterDummy : IRegister
	{
		public (string salt, string hash) GetHashInformation(string email)
		{
			return ("i443zd2/bhb1+3BTfSInmQ==", "EV9WfZRArzpxCSQ/gK4aOW2/wi+ICztSg3o59Pw9k+A=");
		}

		public bool IsAdmin(string email)
		{

			return true; 
		}

		public bool isBetaUser(string email)
		{

			return false; 
		}

		public void LogAccess(string email)
		{
			
		}

		public bool NoEmailExists(string email)
		{
			if (email == "111@gmail.com")
			{
				return true;
			} else
			{
				return false; 
			}
		}

		public void SaveInfo(string email, string hashed, string salt)
		{
			
		}

		public void UpdateEmail(string OldEmail, string NewEmail)
		{
			
		}

		public void UpdatePassword(string Email, string Password, string salt)
		{
			
		}
	}
}
