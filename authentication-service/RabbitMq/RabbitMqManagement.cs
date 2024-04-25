using authentication_service.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace authentication_service.RabbitMq
{
	public class RabbitMqManagement : IRabbitMqManagement
	{
		public void SendMessage(RabbitMqMessageModel message) {
			var factory = new ConnectionFactory { HostName = "localhost" };
			using var connection = factory.CreateConnection();
			using var channel = connection.CreateModel();

			channel.QueueDeclare(queue: "AuthenticationServiceQueue",
								 durable: false,
								 exclusive: false,
								 autoDelete: false,
								 arguments: null);

			var json = JsonConvert.SerializeObject(message);
			var body = Encoding.UTF8.GetBytes(json);


				channel.BasicPublish(exchange: string.Empty,
									 routingKey: "AuthenticationServiceQueue",
									 basicProperties: null,
									 body: body);
		}
	}
}
