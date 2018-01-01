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

        public void Init()
        {
            //_Adapter.Init(ip, port, userName, password, heartbeat);
            _Adapter.Connect();
            _Adapter.MessageReceivedII += OnMessageReceived;
        }

        public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string queue = $"MQ{DateTime.Now.ToString("yyyyMMdd")}.ReplyMessage";
            _Adapter.Publish(queue, e.Message + "成功收到了");
            Console.WriteLine("OnMessageReceived收到訊息: " + e.Message + " 時間: " + DateTime.Now.ToLongDateString());
        }

        public void Shutdown()
        {
            if (_Adapter == null) return; else _Adapter.Disconnect();
        }

        public void Start()
        {
            _Adapter.Comsume();
        }

        public void StartAsync()
        {
            _Adapter.StartAsync(this);
        }

        public string StartDequeue()
        {
            return _Adapter.StartDequeue();
        }
    }
}
