using Newtonsoft.Json;
using RMQ.Core.MicroService;
using RMQ.Core.Producer;
using RMQ.Core.DTO;
using RMQ.Utility.Nlog;
using RMQ.WinService.Core.Engine.Adapter;
using System;


namespace RMQ.WinService.Core.Engine.Module
{
    public class ScheduleTaskModule
    {
        
        private static ScheduleTaskService _service;
        private static ScheduleTaskService ScheduleTaskService
        {
            get
            {
                if (_service == null)
                    _service = new ScheduleTaskService();
                return _service;
            }
        }

        internal static void Start(int id, string description)
        {
            //寫入log或是DB設定工作狀態
            NLogService.Instance.Info($"{DateTime.Now} Info: ScheduleTaskService()啟動。Id: {id}。Description= {description}");
        }

        internal static void Complete(int id, string v)
        {
            //更新-完成
            NLogService.Instance.Info($"{DateTime.Now} Info: Complete()啟動。id: {id}。v= {v}");
        }

        internal static ScheduleTask GetNextScheduleTask(ScheduleType pushMessage)
        {
            ScheduleTask task = null;
            string queueName = $"MQ{DateTime.Now.ToString("yyyyMMdd")}.TaskQueue";
            

            try
            {
                //***************改寫成RabbitMQ 的 Consumer 區塊**************
                //Receiver = new PushNotificationService("MQ20171212.TaskQueue", 60);
                //Receiver.Init();
                //**********************失敗*********************************


                //***************OptionII************************************
                //if (!RabbitMQConnection.Instance.IsConnected)
                //    RabbitMQConnection.Instance.Connect("127.0.0.1", 5672, "guest", "guest");
                //string returnMessage = "";
                //BasicDeliverEventArgs args;
                //RabbitMQConnection.Instance.ReceiveDequeue(out returnMessage, out args);//使用舊方法去讀取Message
                //if (!string.IsNullOrEmpty(RabbitMQConnection.Instance.Messages))
                //{
                //    //這後這邊要改成序列畫，存進去的值就是一整個JSON格式
                //    task = new ScheduleTask();
                //    task.ScheduleData = RabbitMQConnection.Instance.Messages;
                //}
                ////RabbitMQConnection.Instance.Dispose();
                //**************************成功******************************

                //***************OptionIII 讀取單線程那版的架構方法()***********
                IMQConsumerFacade<PushNotificationService> adapter = new MQConsumerFacade<PushNotificationService>(queueName, 60, 10, false, null, 2, 10);
                adapter.Init("localhost", 5672, "guest", "guest", 30);
                adapter.Connect();
                task = JsonConvert.DeserializeObject<ScheduleTask>(adapter.StartDequeue());
                adapter.Disconnect();
                //************************************************************
                NLogService.Instance.Info($"{DateTime.Now} Info: GetNextScheduleTask()取得訊息。Queue: {queueName}。Task= {task.ScheduleData}");


            }
            catch (Exception ex)
            {
                
                task = null;
            }

            return task;
        }

        internal static void Fail(int id, string v)
        {
            //更新-失敗
            NLogService.Instance.Info($"{DateTime.Now} Info: Fail()啟動。id: {id}。v= {v}");
        }

        internal static void NoMessage(int intervalSec)
        {
            NLogService.Instance.Info($"{DateTime.Now} Info: NoMessage()休息{intervalSec}秒");
        }
    }
}
