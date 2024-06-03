using Npgsql;
using authentication_service.DAL;
using authentication_service.Interfaces;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Data;
using System.Text.Json;

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

		public Boolean isBetaUser(string email)
		{
			bool betaUser = false;

			con.Open();

			using (var command = new NpgsqlCommand("SELECT beta_user FROM hash_storage WHERE email = @Email", con.GetConnectionString()))
			{
				command.Parameters.AddWithValue("Email", email);

				using (var reader = command.ExecuteReader())
				{
					if (reader.Read())
					{
						betaUser = reader.GetBoolean(0);
					}
				}
			}

			con.Close();

			return betaUser;
		}

		public string CreateBetaUsersBatch(double targetPercentage)
		{
			con.Open();

			// Calculate date one year ago
			DateTime oneYearAgo = DateTime.Now.AddYears(-1);

			// Calculate date one month ago
			DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);

			// Query to select users based on criteria
			string sql = "SELECT id, email, beta_user FROM hash_storage WHERE account_created_at >= @oneYearAgo AND last_accessed >= @oneMonthAgo";

			// Prepare the command
			NpgsqlCommand command = new NpgsqlCommand(sql, con.GetConnectionString());
			command.Parameters.AddWithValue("@oneYearAgo", oneYearAgo);
			command.Parameters.AddWithValue("@oneMonthAgo", oneMonthAgo);

			// Execute the query and read the result
			NpgsqlDataReader reader = command.ExecuteReader();

			List<Tuple<long, string, bool>> selectedUsers = new List<Tuple<long, string, bool>>();

			while (reader.Read())
			{
				selectedUsers.Add(new Tuple<long, string, bool>(reader.GetInt64(0), reader.GetString(1), reader.GetBoolean(2)));
			}

			// Close the reader
			reader.Close();

			// Calculate the current percentage of beta users
			int currentBetaUserCount = selectedUsers.Count(u => u.Item3);
			double currentPercentage = (double)currentBetaUserCount / selectedUsers.Count * 100;

			// Calculate the number of users to set or unset as beta users
			int targetBetaUserCount = (int)(selectedUsers.Count * (targetPercentage / 100));
			int betaUserCountDifference = targetBetaUserCount - currentBetaUserCount;

			Random random = new Random();

			if (betaUserCountDifference > 0)
			{
				// Increase the number of beta users
				List<Tuple<long, string, bool>> nonBetaUsers = selectedUsers.Where(u => !u.Item3).ToList();
				HashSet<int> selectedIndexes = new HashSet<int>();

				while (selectedIndexes.Count < betaUserCountDifference && selectedIndexes.Count < nonBetaUsers.Count)
				{
					int index = random.Next(nonBetaUsers.Count);
					selectedIndexes.Add(index);
				}

				foreach (int index in selectedIndexes)
				{
					long userId = nonBetaUsers[index].Item1;
					string updateSql = "UPDATE hash_storage SET beta_user = true WHERE id = @userId";
					NpgsqlCommand updateCommand = new NpgsqlCommand(updateSql, con.GetConnectionString());
					updateCommand.Parameters.AddWithValue("@userId", userId);
					updateCommand.ExecuteNonQuery();
				}
			}
			else if (betaUserCountDifference < 0)
			{
				// Decrease the number of beta users
				List<Tuple<long, string, bool>> currentBetaUsers = selectedUsers.Where(u => u.Item3).ToList();
				betaUserCountDifference = Math.Abs(betaUserCountDifference);
				HashSet<int> selectedIndexes = new HashSet<int>();

				while (selectedIndexes.Count < betaUserCountDifference && selectedIndexes.Count < currentBetaUsers.Count)
				{
					int index = random.Next(currentBetaUsers.Count);
					selectedIndexes.Add(index);
				}

				foreach (int index in selectedIndexes)
				{
					long userId = currentBetaUsers[index].Item1;
					string updateSql = "UPDATE hash_storage SET beta_user = false WHERE id = @userId";
					NpgsqlCommand updateCommand = new NpgsqlCommand(updateSql, con.GetConnectionString());
					updateCommand.Parameters.AddWithValue("@userId", userId);
					updateCommand.ExecuteNonQuery();
				}
			}

			// Query to select all users who are now beta users
			string selectBetaUsersSql = "SELECT email FROM hash_storage WHERE beta_user = true";
			NpgsqlCommand selectBetaUsersCommand = new NpgsqlCommand(selectBetaUsersSql, con.GetConnectionString());
			NpgsqlDataReader betaUserReader = selectBetaUsersCommand.ExecuteReader();

			// Collect the result emails into a list
			List<string> resultEmails = new List<string>();
			while (betaUserReader.Read())
			{
				resultEmails.Add(betaUserReader.GetString(0));
			}

			// Close the reader and connection
			betaUserReader.Close();
			con.Close();

			string result = JsonSerializer.Serialize(resultEmails);

			return result;
		} 
	}
}
