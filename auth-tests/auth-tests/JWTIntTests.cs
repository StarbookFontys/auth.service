using auth_tests.DAL;
using auth_tests.RabbitMq;
using authentication_service.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using authentication_service.Models;
using authentication_service.Controllers;
using Microsoft.Extensions.Configuration;
using System.Transactions;
using System.Xml.XPath;
using System.Xml;
using authentication_service.Enums;

namespace auth_tests
{
	[TestClass]
	public class JWTIntTests
	{
		private readonly AccountManagement accountManagement;
		private readonly RegisterDummy registerDummy;
		private readonly Unregisterdummy unregisterDummy;
		private readonly byte[] samplesalt;
		private readonly HashInfo samplehash;
		private readonly string samplepassword;
		private readonly Rabbitmqdummy rabbitmqdummy;
		private readonly JWTManagement jwtmanagement;

		public JWTIntTests()
		{
			registerDummy = new RegisterDummy();
			unregisterDummy = new Unregisterdummy();
			rabbitmqdummy = new Rabbitmqdummy();
			jwtmanagement = new JWTManagement("very_secret_key_that_is_very_secret", "I refuse");
			accountManagement = new AccountManagement(unregisterDummy, registerDummy, rabbitmqdummy, jwtmanagement);
			samplesalt = Convert.FromBase64String("i443zd2/bhb1+3BTfSInmQ==");
			samplehash = new HashInfo
			{
				email = "111@gmail.com",
				hashed = "EV9WfZRArzpxCSQ/gK4aOW2/wi+ICztSg3o59Pw9k+A=",
				salt = Convert.FromBase64String("i443zd2/bhb1+3BTfSInmQ==")
			};
		}

		[TestMethod]
		public void JWTTest()
		{
			string generated = jwtmanagement.GenerateJWTToken("email@gmail.com", "True", false);
			Assert.IsTrue(jwtmanagement.ValidateToken(generated));
		}
	}
}
