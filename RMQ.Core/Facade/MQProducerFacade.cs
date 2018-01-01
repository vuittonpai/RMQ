using System;
using RMQ.Core.EventArg;
using RMQ.Core.Adapter;

namespace RMQ.Core.Facade
{
    /// <summary>
    /// 這個是給Producer用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MQProducerFacade : IMQProducerFacade
    {
        internal RMQAdapter Adapter { get; set; }
        public MQProducerFacade()
        {
            Adapter = RMQAdapter.Instance;
        }
       
        public void Connect()
        {
            Adapter.Connect();
        }
       
        public void Init()
        {
            Adapter.Init();
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
        
    }
}
