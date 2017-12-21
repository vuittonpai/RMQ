
using Newtonsoft.Json;
using RMQ.Core.DTO;
using RMQ.Core.EventArg;
using RMQ.Core.Producer;
using System;
using System.Collections.Generic;


namespace RMQ.Core.MicroService
{
    public class PushNotificationService : IMicroService //RMQConsumer
    {
        private IMQConsumerFacade<PushNotificationService> _Adapter;
       // private RMQConsumer _RMQConsumer;
        public PushNotificationService(string queueName, int timeout, ushort prefetchCount = 1
            , bool noAck = false
            , IDictionary<string, object> queueArgs = null
            , int ConsumerNumber = 2, int MessageNumber = 10)
        //: base(queueName, timeout, prefetchCount, noAck, queueArgs)
        {
            //建立Consumer的參數
            _Adapter = new MQConsumerFacade<PushNotificationService>(queueName, timeout, prefetchCount, noAck, queueArgs, ConsumerNumber, MessageNumber);
        }



        //public string returnData { get; set; }

        public void Init(string ip, int port, string userName, string password, ushort heartbeat)
        {
            //_Adapter = RMQAdapter.Instance; A: 改由constructor
            _Adapter.Init(ip, port, userName, password, heartbeat);
            _Adapter.Connect();
            //base.MessageReceived += OnMessageReceived;
       
            _Adapter.MessageReceivedII += OnMessageReceived;
            //base.Start(_adapter);//可改成非同步方式 A: 區分開來可以async，或是單執行續，StartAsync()
        }

        //private void OnMessageReceivedII(object sender, MessageReceivedEventArgs e)
        //{
        //    //轉移商業邏輯II

        //    Console.WriteLine("OnMessageReceivedII收到訊息: " + e?.Message.ToString() + " 時間: " + DateTime.Now.ToLongDateString());

        //}

        public void Start()
        {
            _Adapter.Comsume();
            //base.Start(_Adapter);
        }
        //public void StartAsync()
        //{
        //    _Adapter.StartAsync(this);
        //}
        //public string StartDequeue()
        //{
        //    return _Adapter.StartDequeue();
        //}


        public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            //商業邏輯的專案在這個
            var result = JsonConvert.DeserializeObject<ScheduleTask>(e.Message);
            Console.WriteLine("OnMessageReceived收到訊息: " + result.ScheduleData + " 時間: " + DateTime.Now.ToLongDateString());
        }

        public void Shutdown() 
        {
            if (_Adapter == null) return;

            _Adapter.Disconnect();
        }


    }
}
