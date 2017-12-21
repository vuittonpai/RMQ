
using RabbitMQ.Client;
using RMQ.Core.Consumer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RMQ.Core.Producer
{
    internal abstract class AMQPAdapter : IDisposable
    {
        protected string hostName;
        protected int port;
        protected string userName;
        protected string password;
        protected ushort heartbeat;

        public abstract bool IsConnected { get; }
        public abstract AMQPAdapter Init(string ip, int port, string userName, string password, ushort heartbeat);
        public abstract void Connect();
        public abstract void Disconnect();
        public abstract object GetConnection();
        public abstract void Publish(string message, string queueName, bool createQueue = true,
            IBasicProperties messageProperties = null, IDictionary<string, object> queueArgs = null);

        void IDisposable.Dispose()
        {
            Disconnect();
        }

        /// <summary>
        /// 讓IAsyncMicroService用
        /// 微服務之間的溝通
        /// </summary>
        /// <param name="consumer"></param>
        public void StartAsync(AMQPComsumer consumer)
        {
            if (!IsConnected) Connect();

            var thread = Task.Run(() => consumer.StartAsync(this));
            //consumer.Start(this);//測試用
        }

 


    }
}
