

namespace RMQ.Core.Producer
{
    public interface IMQProducerFacade
    {
        bool IsConnected();
        void Init(string ip, int port, string userName, string password, ushort heartbeat);
        void Connect();        
        void Publish(string queueName, string message);
        void Disconnect();
        string GetReturnMessage(string replyQueue);

    }
}
