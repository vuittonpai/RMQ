
using RabbitMQ.Client.Events;
using RMQ.Core.Adapter;
using RMQ.Core.EventArg;
using System;
using System.Collections.Generic;

namespace RMQ.Core.Consumer
{
    public abstract class AMQPComsumer
    {
        protected readonly string queueName;
        protected readonly ushort prefetchCount;
        protected readonly bool noAck;
        protected readonly int timeout;
        protected readonly IDictionary<string, object> queueArgs;
        protected volatile bool stopConsuming = false;


        protected AMQPComsumer(string queueName, int timeout, ushort prefetchCount = 1, bool noAck = false,
             IDictionary<string, object> queueArgs = null)
        {
            this.queueName = queueName;
            this.prefetchCount = prefetchCount;
            this.noAck = noAck;
            this.timeout = timeout;//The timeout is in millseconds, 給Dequeue mehtod 用
            this.queueArgs = queueArgs;//預留
        }

        internal abstract void Start(AMQPAdapter amqpAdapter);
        internal abstract void StartAsync(AMQPAdapter amqpAdapter);
        internal abstract string StartDequeue(AMQPAdapter amqpAdapter); 
        
    }
}
