using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace auth_tests
{
	internal class Cleanupe2e
	{
		private NpgsqlConnection _connection;
		private string _ConnectionString;

		public Cleanupe2e(string connectionString) {
			_ConnectionString = connectionString;
		}

		public void RemoveInformation(string email)
		{
			Open();

			NpgsqlCommand cmd = new NpgsqlCommand();

			cmd.Connection = _connection;
			cmd.CommandText = "DELETE FROM hash_storage WHERE email = @Email";
			cmd.Parameters.AddWithValue("Email", email);
			cmd.ExecuteNonQuery();

			Close();
		}

		public void Open()
		{
			_connection = new NpgsqlConnection(_ConnectionString);
			_connection.Open();
		}

		public void Close()
		{
			if (_connection != null && _connection.State != System.Data.ConnectionState.Closed)
			{
				_connection.Close();
			}
		}
	}
}
