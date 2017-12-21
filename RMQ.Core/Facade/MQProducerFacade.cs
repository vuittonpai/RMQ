using System;
using RMQ.Core.EventArg;

namespace RMQ.Core.Producer
{
    /// <summary>
    /// 這個是給Producer用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MQProducerFacade : IMQProducerFacade
    {
        public RMQAdapter Adapter { get; set; }
        public MQProducerFacade()
        {
            Adapter = RMQAdapter.Instance;
        }
       
        public void Connect()
        {
            Adapter.Connect();
        }
       
        public void Init(string ip, int port, string userName, string password, ushort heartbeat)
        {
            Adapter.Init(ip, port, userName, password, heartbeat);
        }

        public bool IsConnected()
        {
            return Adapter.IsConnected;
        }
    
        public void Publish(string queueName, string message)
        {
            Adapter.Publish(queueName, message);
        }

        public void Disconnect()
        {
            Adapter.Disconnect();
        }

        public string GetReturnMessage(string queueName)
        {
            return Adapter.GetReturnMessage(queueName);
        }

        public void GetReturnMessage()
        {
            throw new NotImplementedException();
        }
    }
}
