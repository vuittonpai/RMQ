using Newtonsoft.Json;
using RMQ.Core.Facade;
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
                //***************OptionIII 讀取單線程那版的架構方法()***********
                IMQConsumerFacade<string> adapter = new MQConsumerFacade<string>(queueName, 60, 10, false, null, 2, 10);
                adapter.Init();
                adapter.Connect();
                task = JsonConvert.DeserializeObject<ScheduleTask>(adapter.StartDequeue());
                adapter.Disconnect();

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
