
using RabbitMQ.Client;
using RMQ.Core.Consumer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RMQ.Core.Adapter
{
    internal abstract class AMQPAdapter : IDisposable
    {
        protected string hostName;
        protected int port;
        protected string userName;
        protected string password;
        protected ushort heartbeat;
        /// <summary>
        /// 是否連線
        /// </summary>
        public abstract bool IsConnected { get; }
        /// <summary>
        /// 抽象化，當個別服務需要自訂自己的連線位置跟帳密
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="heartbeat"></param>
        /// <returns></returns>
        public abstract AMQPAdapter Init(string ip, int port, string userName, string password, ushort heartbeat);
        /// <summary>
        /// 預留
        /// </summary>
        /// <returns></returns>
        public abstract AMQPAdapter Init();
        /// <summary>
        /// HA方式連線至多台Queue機器
        /// </summary>
        public abstract void Connect();
        /// <summary>
        /// 關閉連線
        /// </summary>
        public abstract void Disconnect();
        /// <summary>
        /// 取得連線物件
        /// </summary>
        /// <returns></returns>
        public abstract object GetConnection();
        /// <summary>
        /// Producer端發送訊息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="queueName"></param>
        /// <param name="createQueue"></param>
        /// <param name="messageProperties"></param>
        /// <param name="queueArgs"></param>
        public abstract void Publish(string message, string queueName, bool createQueue = true,
            IBasicProperties messageProperties = null, IDictionary<string, object> queueArgs = null);
        /// <summary>
        /// Dispose
        /// </summary>
        void IDisposable.Dispose()
        {
            Disconnect();
        }
        /// <summary>
        /// 使用背景作業方式Consumer
        /// 微服務之間的溝通
        /// </summary>
        /// <param name="consumer"></param>
        public void StartAsync(AMQPComsumer consumer)
        {
            if (!IsConnected) Connect();

            if (Debugger.IsAttached)
                consumer.Start(this);//測試用
            else
            {
                var thread = Task.Run(() => consumer.StartAsync(this));
            }
        }
    }
}
