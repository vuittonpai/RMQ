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
        /// <summary>
        /// 啟動Consumer
        /// </summary>
        public void Comsume()
        {
            base.Start(Adapter);
        }
        /// <summary>
        /// 觸發機制
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceivedII;
        /// <summary>
        /// 將訊息往前丟，搭配封裝內容模式
        /// </summary>
        public new void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            MessageReceivedII?.Invoke(this, e);
        }
        /// <summary>
        /// 建立連線
        /// </summary>
        public void Connect()
        {
            Adapter.Connect();
        }
        /// <summary>
        /// 是否連線
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return Adapter.IsConnected;
        }
        /// <summary>
        /// Producer端發送訊息
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="message"></param>
        public void Publish(string queueName, string message)
        {
            Adapter.Publish(queueName, message);
        }
        /// <summary>
        /// 啟用服務之間溝通
        /// </summary>
        /// <param name="microserviceService"></param>
        public void StartAsync(T microserviceService)
        {            
            Adapter.StartAsync(this);
        }
        /// <summary>
        /// 單一程序取得Q訊息用
        /// </summary>
        /// <returns></returns>
        public string StartDequeue()
        {
            return base.StartDequeue(Adapter);
        }
        /// <summary>
        /// 關閉連線
        /// </summary>
        public void Disconnect()
        {
            Adapter.Disconnect();
        }
                
    }
}
