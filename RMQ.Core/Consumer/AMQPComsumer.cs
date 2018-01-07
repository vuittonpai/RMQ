
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

        /// <summary>
        /// Consumer建構式
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="timeout"></param>
        /// <param name="prefetchCount"></param>
        /// <param name="noAck"></param>
        /// <param name="queueArgs"></param>
        protected AMQPComsumer(string queueName, int timeout, ushort prefetchCount = 1, bool noAck = false,
             IDictionary<string, object> queueArgs = null)
        {
            this.queueName = queueName;
            this.prefetchCount = prefetchCount;
            this.noAck = noAck;
            this.timeout = timeout;//The timeout is in millseconds, 給Dequeue mehtod 用
            this.queueArgs = queueArgs;//預留，可做對Queue的操作設定
        }
        /// <summary>
        /// 有輪圈功能的Consumer
        /// </summary>
        /// <param name="amqpAdapter"></param>
        internal abstract void Start(AMQPAdapter amqpAdapter);
        /// <summary>
        /// 背景Consumer處理服務之間的溝通
        /// </summary>
        /// <param name="amqpAdapter"></param>
        internal abstract void StartAsync(AMQPAdapter amqpAdapter);
        /// <summary>
        /// 單一程序取得Q訊息用
        /// </summary>
        /// <param name="amqpAdapter"></param>
        /// <returns></returns>
        internal abstract string StartDequeue(AMQPAdapter amqpAdapter); 
        
    }
}
