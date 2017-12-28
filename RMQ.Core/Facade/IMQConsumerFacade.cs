using RMQ.Core.EventArg;
using System;

namespace RMQ.Core.Facade
{
    public interface IMQConsumerFacade<T>
    {
        bool IsConnected();
        void Init();
        void Connect();
        void Comsume();
        void Publish(string queueName, string message);
        void StartAsync(T microService);
        string StartDequeue();
        void Disconnect();
   
        event EventHandler<MessageReceivedEventArgs> MessageReceivedII;
        void OnMessageReceived(object sender, MessageReceivedEventArgs e);
    }
}
