
using RabbitMQ.Client;
using RMQ.Adapter.Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RMQ.Adapter.Producer
{
    public abstract class AMQPAdapter : IDisposable
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
        //public abstract void AcknowledgeMessage(ulong deliveryTag);
        void IDisposable.Dispose()
        {
            Disconnect();
        }


        protected object _Lock = new object();
        /// <summary>
        /// 讓Windows Service去抓
        /// </summary>
        /// <param name="consumer"></param>
        public void StartAsync(AMQPComsumer consumer)
        {
            if (!IsConnected) Connect();

            var thread = Task.Run(() => consumer.StartAsync(this));
            // var thread = new Thread(o => consumer.StartAsync(this));
            //thread.Start();
            //while (!thread.IsAlive)
            //{
            //    Thread.Sleep(1);
            //}
        }

 


    }
}
