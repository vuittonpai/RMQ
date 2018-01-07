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
        /// <summary>
        /// 紀錄工作狀態
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        internal static void Start(int id, string description)
        {
            NLogService.Instance.Info($"{DateTime.Now} Info: ScheduleTaskService()啟動。Id: {id}。Description= {description}");
        }
        /// <summary>
        /// 紀錄完成
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        internal static void Complete(int id, string description)
        {
            NLogService.Instance.Info($"{DateTime.Now} Info: Complete()啟動。id: {id}。Description= {description}");
        }
        /// <summary>
        /// 取得Queue內的推波訊息
        /// </summary>
        /// <param name="pushMessage"></param>
        /// <returns></returns>
        internal static ScheduleTask GetNextScheduleTask(ScheduleType pushMessage)
        {
            ScheduleTask task = null;
            string queueName = $"MQ{DateTime.Now.ToString("yyyyMMdd")}.TaskQueue";

            try
            {
                //***************OptionIII 讀取單線程那版的架構方法()***********
                IMQConsumerFacade<string> adapter = new MQConsumerFacade<string>(queueName, 60, 10, false, null, 2, 10);
                //adapter.Init();
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
        /// <summary>
        /// 紀錄失敗
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        internal static void Fail(int id, string description)
        {
            NLogService.Instance.Info($"{DateTime.Now} Info: Fail()啟動。id: {id}。Description= {description}");
        }
        /// <summary>
        /// 紀錄無訊息狀態
        /// </summary>
        /// <param name="intervalSec"></param>
        internal static void NoMessage(int intervalSec)
        {
            NLogService.Instance.Info($"{DateTime.Now} Info: NoMessage()啟動。休息{intervalSec}秒");
        }
        /// <summary>
        /// 紀錄錯誤訊息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="v"></param>
        internal static void Exception(int id, string description)
        {
            NLogService.Instance.Error($"{DateTime.Now} Error: Exception()發生。id: {id}。Description= {description}");
        }
    }
}
