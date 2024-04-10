using Npgsql;
using authentication_service.DAL;
using authentication_service.Interfaces;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

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
		}

		public (string salt, string hash) GetHashInformation(string email)
		{
			string salt = "null";
			string hash = "null"; 
			try
			{
				con.Open();

				string insertQuery = "SELECT salt, hash FROM hash_storage WHERE email = @Email";

				using var GetHashCmd = new NpgsqlCommand(insertQuery, con.GetConnectionString());
				GetHashCmd.Parameters.AddWithValue("@Email", email);
				using (NpgsqlDataReader reader = GetHashCmd.ExecuteReader())
				{
					if (reader.Read())
					{
						salt = reader.GetString(0);
						hash = reader.GetString(1);
					}
				}
				con.Close();
				return (salt, hash);
			}
			catch(NpgsqlException ex)
			{
				return (salt, hash); 
			}
		}
	}
}
