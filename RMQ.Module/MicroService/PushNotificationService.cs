
using Newtonsoft.Json;
using RMQ.Core.DTO;
using RMQ.Core.EventArg;
using RMQ.Core.Producer;
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
                
        public void Init(string ip, int port, string userName, string password, ushort heartbeat)
        {
           
            _Adapter.Init(ip, port, userName, password, heartbeat);
            _Adapter.Connect();
            _Adapter.MessageReceivedII += OnMessageReceived;
        }

        public void Start()
        {
            _Adapter.Comsume();
        }

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
