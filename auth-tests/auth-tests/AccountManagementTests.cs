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
		//private readonly HashInfo samplehash;
		//private readonly UserInfo sampleuserinfo;
		//private readonly string samplepassword;
		private readonly Rabbitmqdummy rabbitmqdummy;
		private readonly JWTManagement jwtmanagement;

		public AccountManagementTests() 
		{
			registerDummy = new RegisterDummy();
			unregisterDummy = new Unregisterdummy();
			rabbitmqdummy = new Rabbitmqdummy();
			jwtmanagement = new JWTManagement("no", "I refuse");
			accountManagement = new AccountManagement(unregisterDummy, registerDummy, rabbitmqdummy, jwtmanagement);
			samplesalt = Convert.FromBase64String("i443zd2/bhb1+3BTfSInmQ==");
			//sampleuserinfo = new UserInfo
			//{
			//	email = "111@gmail.com",
			//	password = "111"
			//};
			//samplehash = new HashInfo
			//{
			//	email = "111@gmail.com",
			//	hashed = "EV9WfZRArzpxCSQ/gK4aOW2/wi+ICztSg3o59Pw9k+A=",
			//	salt = Convert.FromBase64String("i443zd2/bhb1+3BTfSInmQ==")
			//};
		}

		[TestMethod]
		public void TestVerifyAccountInformationTrue()
		{
			bool actual = accountManagement.VerifyInformation("111@gmail.com", "111");

			Assert.IsTrue(actual);
		}

		[TestMethod]
		public void TestVerifyAccountINformationFalse()
		{
			bool actual = accountManagement.VerifyInformation("111@gmail.com", "WRONGPASSWORD");

			Assert.IsFalse(actual);
		}

		[TestMethod]
		public void TestUpdateEmail()
		{
			accountManagement.UpdateEmail("111@gmail.com", "222@gmail.com");
		}

		[TestMethod]
		public void TestUpdatePassword()
		{
			accountManagement.UpdatePassword("111@gmail.com", "111");
		}

		[TestMethod]
		public void TestDeleteInformationTrue()
		{
			accountManagement.DeleteInformation("111@gmail.com", "111");
		}

		[TestMethod]
		[ExpectedException(typeof(authentication_service.Exceptions.PasswordIncorrectEx))]
		public void TestDeleteInformationFalse()
		{
			accountManagement.DeleteInformation("111@gmail.com", "222");
		}
	}
}