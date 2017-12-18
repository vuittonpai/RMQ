using System;
using RMQ.Adapter.EventArg;

namespace RMQ.Core.Producer
{
    public interface IMessageQueueAdapter<T>
    {
        bool IsConnected();
        void Init(string ip, int port, string userName, string password, ushort heartbeat);
        void Connect();
        void Comsume();
        void Publish(string queueName, string message);
        void StartAsync(T microService);
        string StartDequeue();
        void Disconnect();
   
        event EventHandler<MessageReceivedEventArgs> moveMessageLogicToFront;
    }
}
