using Npgsql;
using authentication_service.DAL;
using authentication_service.Interfaces;
using System.Runtime.CompilerServices;

namespace authentication_service.DAL
{
	public class Register : IRegister
	{
		private readonly IConnection con;

		public Register(IConnection _con)
		{
			con = _con;
		}

		public Boolean NoEmailExists(string email)
		{
			con.Open();
			Boolean emailExists; 

			NpgsqlCommand cmd = new NpgsqlCommand();

			cmd.Connection = con.GetConnectionString();
			cmd.CommandText = "SELECT COUNT(*) FROM hash_storage WHERE email = @Email";
			cmd.Parameters.AddWithValue("Email", email);
			object result = cmd.ExecuteScalar();
			int count = Convert.ToInt32(result);
			if (count > 0)
			{
				emailExists = true;
			} else{
				emailExists = false; 
			}
			con.Close();
			return emailExists;
			
		}

		public void SaveInfo(string email, string hashed, byte[] salt)
		{
			string EmailError = "Email already exists in the system.";
			try
			{
				con.Open();
				string insertQuery = "INSERT INTO hash_storage (email, salt, hash) VALUES  (@email, @salt, @hash)";
				using var HashCmd = new NpgsqlCommand(insertQuery, con.GetConnectionString());
				HashCmd.Parameters.AddWithValue("@email", email);
				HashCmd.Parameters.AddWithValue("@salt", salt);
				HashCmd.Parameters.AddWithValue("@hash", hashed);
				HashCmd.ExecuteNonQuery();

				con.Close();
			}
			catch(NpgsqlException ex)
			{
				Console.WriteLine("encountered an exception from database. Please check the connection and try again" + ex);
			}
			catch (Exception ex)
			{
				if(ex.Message == EmailError)
				{
					Console.WriteLine("Email already exists in the system");
				}
				Console.WriteLine("Encountered unhandled exception " + ex.Message);
			}
		}

		public string GetHash(string email)
		{
			return "hi";
		}
	}
}
