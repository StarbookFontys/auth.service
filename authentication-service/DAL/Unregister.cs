using authentication_service.Interfaces;
using Npgsql; 

namespace authentication_service.DAL
{
	public class Unregister : IUnregister
	{
		private readonly IConnection con;

		public Unregister(IConnection _con)
		{
			con = _con;
		}

		public void RemoveUserData(string email)
		{
			con.Open();

			NpgsqlCommand cmd = new NpgsqlCommand();

			cmd.Connection = con.GetConnectionString();
			cmd.CommandText = "DELETE FROM hash_storage WHERE email = @Email";
			cmd.Parameters.AddWithValue("Email", email);
			cmd.ExecuteNonQuery();

			con.Close();
		}
	}
}
