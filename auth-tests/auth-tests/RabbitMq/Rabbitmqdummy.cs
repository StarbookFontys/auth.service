using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using authentication_service.Interfaces;
using authentication_service.RabbitMq;

namespace auth_tests.RabbitMq
{
	internal class Rabbitmqdummy : IRabbitMqManagement
	{
		public void SendMessage(RabbitMqMessageModel message)
		{
			
		}
	}
}
