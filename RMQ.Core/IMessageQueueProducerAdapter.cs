using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMQ.Core.Producer
{
    public interface IMessageQueueProducerAdapter
    {
        bool IsConnected();
        void Init(string ip, int port, string userName, string password, ushort heartbeat);
        void Connect();        
        void Publish(string queueName, string message);
        void Disconnect();

    }
}
