using Npgsql;
using authentication_service.DAL;
using authentication_service.Interfaces;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Data;

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

		public void SaveInfo(string email, string hashed, string salt)
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

		public void UpdateEmail(string OldEmail, string NewEmail)
		{
			con.Open();

			NpgsqlCommand cmd = new NpgsqlCommand();

			cmd.Connection = con.GetConnectionString();
			string updateQuery = "UPDATE hash_storage SET email = @NewEmail WHERE email = @OldEmail";
			cmd.CommandText = updateQuery;
			cmd.Parameters.AddWithValue("@OldEmail", OldEmail);
			cmd.Parameters.AddWithValue("NewEmail", NewEmail);
			cmd.ExecuteNonQuery();

			con.Close();
		}

		public void UpdatePassword(string Email,  string Hash, string Salt)
		{
			con.Open();

			NpgsqlCommand cmd = new NpgsqlCommand();
			cmd.Connection = con.GetConnectionString();
			string updateQuery = "UPDATE hash_storage SET hash = @NewHash, salt = @NewSalt WHERE email = @Email";
			cmd.CommandText = updateQuery;
			cmd.Parameters.AddWithValue("@NewHash", Hash);
			cmd.Parameters.AddWithValue("@NewSalt", Salt);
			cmd.Parameters.AddWithValue("@Email", Email);
			cmd.ExecuteNonQuery();
			con.Close();
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

		public Boolean IsAdmin(string email)
		{
			string role = "user"; // Default role if email not found or error occurs

			con.Open();

			// Prepare SQL command
			string sql = "SELECT role FROM hash_storage WHERE email = @Email";
			using (NpgsqlCommand command = new NpgsqlCommand(sql, con.GetConnectionString()))
			{
				// Add parameter for email
				command.Parameters.AddWithValue("@Email", email);

				// Execute SQL command and get the result
				using (NpgsqlDataReader reader = command.ExecuteReader())
				{
					// Check if any rows returned
					if (reader.Read())
					{
						// Get the role value from the result
						role = reader.GetString(reader.GetOrdinal("role"));
					}
				}
			}
			con.Close();

			if (role == "admin")
			{
				return true;
			}
			else
			{
				return false; 
			}
		}

		public void LogAccess(string email)
		{
			string query = "UPDATE hash_storage SET last_accessed = @currentDateTime WHERE email = @email";


			con.Open();

			using (NpgsqlCommand command = new NpgsqlCommand(query, con.GetConnectionString()))
			{
				command.Parameters.AddWithValue("currentDateTime", DateTime.Now);
				command.Parameters.AddWithValue("email", email);

				int rowsAffected = command.ExecuteNonQuery();
			}

			con.Close();
		}

		public string BetaUsers(double percentage)
		{
			con.Open();

			// Calculate date one year ago
			DateTime oneYearAgo = DateTime.Now.AddYears(-1);

			// Calculate date one month ago
			DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);

			// Query to select users based on criteria
			string sql = "SELECT id, email FROM hash_storage WHERE account_created_at >= @oneYearAgo AND last_accessed >= @oneMonthAgo";

			// Prepare the command
			NpgsqlCommand command = new NpgsqlCommand(sql, con.GetConnectionString());
			command.Parameters.AddWithValue("@oneYearAgo", oneYearAgo);
			command.Parameters.AddWithValue("@oneMonthAgo", oneMonthAgo);

			// Execute the query and read the result
			NpgsqlDataReader reader = command.ExecuteReader();

			List<Tuple<long, string>> selectedUsers = new List<Tuple<long, string>>();

			while (reader.Read())
			{
				selectedUsers.Add(new Tuple<long, string>(reader.GetInt64(0), reader.GetString(1)));
			}

			// Close the reader
			reader.Close();

			// Calculate the number of users to set as beta users based on percentage
			int betaUserCount = (int)(selectedUsers.Count * (percentage / 100));

			// Randomly select users to set as beta users
			Random random = new Random();
			HashSet<int> selectedIndexes = new HashSet<int>();

			while (selectedIndexes.Count < betaUserCount)
			{
				int index = random.Next(selectedUsers.Count);
				selectedIndexes.Add(index);
			}

			// Update the selected users to set beta_user flag to true
			foreach (int index in selectedIndexes)
			{
				long userId = selectedUsers[index].Item1;
				string updateSql = "UPDATE hash_storage SET beta_user = true WHERE id = @userId";
				NpgsqlCommand updateCommand = new NpgsqlCommand(updateSql, con.GetConnectionString());
				updateCommand.Parameters.AddWithValue("@userId", userId);
				updateCommand.ExecuteNonQuery();
			}

			// Query to select all users who are now beta users
			string selectBetaUsersSql = "SELECT email FROM hash_storage WHERE beta_user = true";
			NpgsqlCommand selectBetaUsersCommand = new NpgsqlCommand(selectBetaUsersSql, con.GetConnectionString());
			NpgsqlDataReader betaUserReader = selectBetaUsersCommand.ExecuteReader();

			// Build the result string
			string result = "";
			while (betaUserReader.Read())
			{
				result += betaUserReader.GetString(0) + "\n";
			}

			// Close the reader and connection
			betaUserReader.Close();
			con.Close();

			return result;
		}
	}
}
