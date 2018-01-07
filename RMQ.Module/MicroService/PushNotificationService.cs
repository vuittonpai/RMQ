
using Newtonsoft.Json;
using RMQ.Core.DTO;
using RMQ.Core.EventArg;
using RMQ.Core.Facade;
using System;
using System.Collections.Generic;


namespace RMQ.Core.MicroService
{
    public class PushNotificationService : IMicroService
    {
        private IMQConsumerFacade<PushNotificationService> _Adapter;

        public PushNotificationService(string queueName, int timeout, ushort prefetchCount = 1
            , bool noAck = false
            , IDictionary<string, object> queueArgs = null
            , int ConsumerNumber = 2, int MessageNumber = 10)
        {
            _Adapter = new MQConsumerFacade<PushNotificationService>(queueName, timeout, prefetchCount, noAck, queueArgs, ConsumerNumber, MessageNumber);
        }
        /// <summary>
        /// 建構連線
        /// 設定回傳方法
        /// </summary>
        public void Connect()
        {
            //_Adapter.Init(ip, port, userName, password, heartbeat);
            _Adapter.Connect();
            _Adapter.MessageReceivedII += OnMessageReceived;
        }
        /// <summary>
        /// 實作回傳方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            //商業邏輯的專案在這個
            var result = JsonConvert.DeserializeObject<ScheduleTask>(e.Message);
            Console.WriteLine("OnMessageReceived收到訊息: " + result.ScheduleData + " 時間: " + DateTime.Now.ToLongDateString());
        }
        /// <summary>
        /// 結束連線      
        /// </summary>
        public void Shutdown() 
        {
            if (_Adapter == null) return;

            _Adapter.Disconnect();
        }
        /// <summary>
        /// 啟用Consumer
        /// </summary>
        public void Start()
        {
            _Adapter.Comsume();
        }


    }
}
