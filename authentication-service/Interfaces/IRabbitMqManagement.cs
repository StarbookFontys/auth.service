using authentication_service.RabbitMq;

namespace authentication_service.Interfaces
{
	public interface IRabbitMqManagement
	{
		public void SendMessage(RabbitMqMessageModel message);
	}
}
