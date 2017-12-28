using System;
using System.Collections.Generic;
using RMQ.Core.Consumer;
using RMQ.Core.EventArg;
using RMQ.Core.Adapter;

namespace RMQ.Core.Facade
{
    public class MQConsumerFacade<T> : RMQConsumer, IMQConsumerFacade<T>
        where T : class
    {
        internal RMQAdapter Adapter { get; set; }
        public MQConsumerFacade
            (string queueName, int timeout, ushort prefetchCount = 1
            , bool noAck = false, IDictionary<string, object> queueArgs = null
            , int ConsumerNumber = 2, int MessageNumber = 10)
        : base(queueName, timeout, prefetchCount, noAck, queueArgs, ConsumerNumber, MessageNumber)
        {
            Adapter = RMQAdapter.Instance;
            base._MessageReceived += OnMessageReceived;
        }

        public void Comsume()
        {
            base.Start(Adapter);
        }
        public event EventHandler<MessageReceivedEventArgs> MessageReceivedII;
        public new void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            //再往外丟
            MessageReceivedII?.Invoke(this, e);
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

        public void StartAsync(T microserviceService)
        {            
            Adapter.StartAsync(this);
        }
        public string StartDequeue()
        {
            return base.StartDequeue(Adapter);
        }

        public void Disconnect()
        {
            Adapter.Disconnect();
        }
                
    }
}
