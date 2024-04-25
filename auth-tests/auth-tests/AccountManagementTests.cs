using authentication_service.Business;
using authentication_service.Interfaces;
using auth_tests.DAL;
using authentication_service.Models;
using auth_tests.TestModels;
using auth_tests.RabbitMq;

namespace auth_tests
{
	[TestClass]
	public class AccountManagementTests
	{
		private readonly AccountManagement accountManagement;
		private readonly RegisterDummy registerDummy;
		private readonly Unregisterdummy unregisterDummy;
		private readonly byte[] samplesalt;
		private readonly HashInfo samplehash;
		private readonly UserInfo sampleuserinfo;
		private readonly string samplepassword;
		private readonly Rabbitmqdummy rabbitmqdummy;

		public AccountManagementTests() 
		{
			registerDummy = new RegisterDummy();
			unregisterDummy = new Unregisterdummy();
			rabbitmqdummy = new Rabbitmqdummy();
			accountManagement = new AccountManagement(unregisterDummy, registerDummy, rabbitmqdummy);
			samplesalt = Convert.FromBase64String("i443zd2/bhb1+3BTfSInmQ==");
			sampleuserinfo = new UserInfo
			{
				email = "111@gmail.com",
				password = "111"
			};
			samplehash = new HashInfo
			{
				email = "111@gmail.com",
				hashed = "EV9WfZRArzpxCSQ/gK4aOW2/wi+ICztSg3o59Pw9k+A=",
				salt = Convert.FromBase64String("i443zd2/bhb1+3BTfSInmQ==")
			};
		}

		[TestMethod]
		public void TestVerifyAccountInformation()
		{
			bool actual = accountManagement.VerifyInformation("111@gmail.com", "111");

			Assert.IsTrue(actual);
		}
	}
}