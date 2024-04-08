using Npgsql;

namespace authentication_service.Interfaces
{
	public interface IConnection
	{
		void Open();
		void Close();
		NpgsqlConnection GetConnectionString();
	}
}
