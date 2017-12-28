

namespace RMQ.Core.Facade
{
    public interface IMQProducerFacade
    {
        bool IsConnected();
        void Init();
        void Connect();        
        void Publish(string queueName, string message);
        void Disconnect();
        string GetReturnMessage(string replyQueue);

    }
}
