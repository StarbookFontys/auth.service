using authentication_service.Business;
using authentication_service.Models;
using System.ComponentModel.DataAnnotations;

namespace auth_tests
{
	[TestClass]
	public class EncryptorTests
	{
		private readonly Encryptor encryptor;
		private readonly byte[] samplesalt;
		private readonly HashInfo samplehash; 
		public EncryptorTests() 
		{ 
			encryptor = new Encryptor();
			samplesalt = Convert.FromBase64String("i443zd2/bhb1+3BTfSInmQ==");
			samplehash = new HashInfo
			{
				email = "111@gmail.com",
				hashed = "EV9WfZRArzpxCSQ/gK4aOW2/wi+ICztSg3o59Pw9k+A=",
				salt = Convert.FromBase64String("i443zd2/bhb1+3BTfSInmQ==")
			};
		}

		[TestMethod]
		public void GeneratesDifferentSalts()
		{
			byte[] expected = samplesalt;
			byte[] actual = encryptor.GenerateSalt();

			Assert.AreNotEqual(expected, actual);
			if(actual.Length != expected.Length)
			{
				Assert.Fail("Expected a different byte array length. Expected: " + Convert.ToString(expected.Length) + ", Actual: " + Convert.ToString(actual.Length));
			}
		}

		[TestMethod]
		public void CompareSaltLengths()
		{
			byte[] expected = samplesalt;
			byte[] actual = encryptor.GenerateSalt();

			if (actual.Length != expected.Length)
			{
				Assert.Fail("Expected a different byte array length. Expected: " + Convert.ToString(expected.Length) + ", Actual: " + Convert.ToString(actual.Length));
			}
		}

		[TestMethod]
		public void CreateHashed()
		{
			//WHEN a new password and email is encrypted
			HashInfo actual = encryptor.Hasher("111@gmail.com", "111", samplesalt);

			//THEN new hash info is created. 
			Assert.IsNotNull(actual);
			Assert.IsNotNull(actual.email);
			Assert.IsNotNull(actual.hashed);
			Assert.IsNotNull(actual.salt);
		}

		[TestMethod]
		public void SimilarHashed()
		{
			HashInfo expected = samplehash;
			HashInfo actual = encryptor.Hasher("111.gmail.com", "111", samplesalt);

			Assert.AreEqual(expected.hashed, actual.hashed);
		}

		public void DifferentHash()
		{
			HashInfo expected = samplehash;
			HashInfo actual = encryptor.Hasher("111.gmail.com", "111", encryptor.GenerateSalt());

			Assert.AreNotEqual(expected, actual);
		}
	}
}