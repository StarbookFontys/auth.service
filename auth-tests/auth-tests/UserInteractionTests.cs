using auth_tests.DAL;
using auth_tests.RabbitMq;
using auth_tests.TestModels;
using authentication_service.Business;
using authentication_service.Controllers;
using authentication_service.DAL;
using authentication_service.Interfaces;
using authentication_service.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_tests
{
	[TestClass]
	public class UserInteractionTests
	{
		private readonly string ConnectionString;
		private readonly AccountManagement accountManagement;
		private readonly Register Register;
		private readonly Unregister Unregister;
		//private readonly byte[] samplesalt;
		//private readonly HashInfo samplehash;
		private readonly TestModels.UserInfo sampleuserinfo;	
		private readonly IConnection con;
		//private readonly string email;
		private readonly Rabbitmqdummy rabbitmqdummy;

		public UserInteractionTests()
		{
			ConnectionString = "Host=firstcluster-14261.8nj.gcp-europe-west1.cockroachlabs.cloud;Port=26257;Database=defaultdb;Password=pM6WlZHYezsnKfLKdW5-Cw;SSL Mode=VerifyFull;Username=coen";

			con = new Connection(ConnectionString);
			Register = new Register(con);
			Unregister = new Unregister(con);
			rabbitmqdummy = new Rabbitmqdummy();
			accountManagement = new AccountManagement(Unregister, Register, rabbitmqdummy);
			//samplesalt = Convert.FromBase64String("i443zd2/bhb1+3BTfSInmQ==");

			sampleuserinfo = new TestModels.UserInfo
			{
				email = "notrealemail",
				password = "111"
			};
			//samplehash = new HashInfo
			//{
			//	email = "notrealemail",
			//	hashed = "EV9WfZRArzpxCSQ/gK4aOW2/wi+ICztSg3o59Pw9k+A=",
			//	salt = Convert.FromBase64String("i443zd2/bhb1+3BTfSInmQ==")
			//};
		}

		[TestMethod]
		public void AccountCreationTest()
		{
			accountManagement.SaveInformation(sampleuserinfo.email, sampleuserinfo.password);
			Assert.IsTrue(accountManagement.VerifyInformation(sampleuserinfo.email, sampleuserinfo.password));
		}

		[TestCleanup]
		public void Cleanup()
		{
			Cleanupe2e CleanupClass = new Cleanupe2e(ConnectionString);
			CleanupClass.RemoveInformation(sampleuserinfo.email);
		}
	}
}
