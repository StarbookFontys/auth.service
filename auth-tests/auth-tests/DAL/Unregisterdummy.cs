using authentication_service.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace auth_tests.DAL
{
	internal class Unregisterdummy : IUnregister
	{
		public string Email = "email@gmail.com";
		public void RemoveUserData(string email)
		{
			if(Email == email)
			{
				email = " ";
			}
		}
	}
}
