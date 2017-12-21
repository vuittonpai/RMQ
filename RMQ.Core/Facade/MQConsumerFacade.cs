using System;
using System.Collections.Generic;
using RMQ.Core.Consumer;
using RMQ.Core.EventArg;

namespace RMQ.Core.Producer
{
    public class MQConsumerFacade<T> : RMQConsumer, IMQConsumerFacade<T>, IDisposable
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
        }

        
        public event EventHandler<MessageReceivedEventArgs> MessageReceivedII;
        //object objectLock = new Object();
        //event EventHandler<MessageReceivedEventArgs> IMQConsumerFacade<T>.moveMessageLogicToFront
        //{
        //    add
        //    {
        //        lock (objectLock)
        //            moveMessageLogicToFront += value;
        //    }
        //    remove
        //    {
        //        lock (objectLock)
        //            moveMessageLogicToFront -= value;
        
        //    }
        //}

        //public void DoBusinessLogic()
        //{
        //    //往外丟觸發處，目前先移到OnMessageReceived()那觸發
        //    EventHandler handler = moveMessageLogicToFront;
        //    if (handler != null)
        //    {
        //        handler(this, new EventArgs());
        //    }
        //}

        public void Comsume()
        {
            base.MessageReceived += OnMessageReceived;
            base.Start(Adapter);
        }
        public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            //再往外丟
            MessageReceivedII?.Invoke(this, e);
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
            Adapter.PublishMessage(queueName, message);
        }

        public void StartAsync(T microserviceService)
        {
            base.MessageReceived += OnMessageReceived;
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

        public void Dispose()
        {
            Adapter.Disconnect();
        }
    }
}
