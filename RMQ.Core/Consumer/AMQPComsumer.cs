
using RabbitMQ.Client.Events;
using RMQ.Core.EventArg;
using RMQ.Core.Producer;
using System;
using System.Collections.Generic;

namespace RMQ.Core.Consumer
{
    public abstract class AMQPComsumer
    {
        protected readonly string queueName;
        protected readonly ushort prefetchCount;
        protected readonly bool noAck;
        //protected readonly bool createQueue;
        protected readonly int timeout;
        protected readonly bool implicitAck;
        protected readonly IDictionary<string, object> queueArgs;
        protected volatile bool stopConsuming = false;


        protected AMQPComsumer(string queueName, int timeout, ushort prefetchCount = 1, bool noAck = false,
             IDictionary<string, object> queueArgs = null)
        {
            this.queueName = queueName;
            this.prefetchCount = prefetchCount;
            this.noAck = noAck;
            //this.createQueue = createQueue;//可移除;
            this.timeout = timeout;//The timeout is in millseconds, 之前是給Dequeue mehtod 用，但我改用新的EventingBasicConsumer去取
            //this.implicitAck = implicitAck;//可移除
            this.queueArgs = queueArgs;//預留
        }

        public delegate string ReturnStringEventHandler(object sender, EventArgs args);
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        //觸發機制，讓前面去實作商業邏輯
        protected void OnMessageReceived(MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
            //var handler = MessageReceived;
            //if (handler != null) handler(this, e);
        }

        //共用Producer內的連線機制
        public abstract void Start(AMQPAdapter amqpAdapter);
        public abstract void StartAsync(AMQPAdapter amqpAdapter);
        public abstract string StartDequeue(AMQPAdapter amqpAdapter); 
        //RabbitMQ的內建監聽機制
        public event EventHandler Consumer_Received;
        protected virtual void OnConsumer_ReceivedII(object sender, BasicDeliverEventArgs e)
        {
            if (this.Consumer_Received != null)
            {
                this.Consumer_Received(this, e);
            }
        }
    }
}
