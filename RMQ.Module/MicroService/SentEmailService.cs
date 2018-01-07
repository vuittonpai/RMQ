using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMQ.Core.EventArg;
using RMQ.Core.Facade;

namespace RMQ.Core.MicroService
{
    public class SentEmailService : IAsyncMicroService
    {
        private IMQConsumerFacade<SentEmailService> _Adapter;
        public SentEmailService(string queueName, int timeout, ushort prefetchCount = 1
            , bool noAck = false, IDictionary<string, object> queueArgs = null
            , int ConsumerNumber = 2, int MessageNumber = 10)
        {
            _Adapter = new MQConsumerFacade<SentEmailService>(queueName, timeout, prefetchCount, noAck, queueArgs, ConsumerNumber, MessageNumber);
        }
        /// <summary>
        /// 建構連線
        /// 設定回傳方法，一定要設定，不然服務之間無法溝通
        /// </summary>
        public void Connect()
        {
            //_Adapter.Init(ip, port, userName, password, heartbeat);
            _Adapter.Connect();
            _Adapter.MessageReceivedII += OnMessageReceived;
        }
        /// <summary>
        /// 回傳方法實作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string queue = $"MQ{DateTime.Now.ToString("yyyyMMdd")}.ReplyMessage";
            _Adapter.Publish(queue, e.Message + "成功收到了");
            Console.WriteLine("OnMessageReceived收到訊息: " + e.Message + " 時間: " + DateTime.Now.ToLongDateString());
        }
        /// <summary>
        /// 啟用服務之間溝通
        /// </summary>
        public void StartAsync()
        {
            _Adapter.StartAsync(this);
        }

    }
}
