using Microsoft.AspNetCore.Connections;
using authentication_service.Interfaces;
using Npgsql;

namespace authentication_service.DAL;

	public class Connection : IConnection
	{
        private NpgsqlConnection _connection;
        private string _ConnectionString;

    public Connection(string connectionString)
    {
        _ConnectionString = connectionString;
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

    public NpgsqlConnection GetConnectionString() { return _connection; }
	}
