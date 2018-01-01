using Newtonsoft.Json;
using RMQ.Core.DTO;
using RMQ.Core.Facade;
using RMQ.Core.MicroService;
using System;
using System.Web.Mvc;

namespace RMQ.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 非同步方式啟另一個consumer服務
        /// </summary>
        /// <returns></returns>
        public JsonResult Consumer()
        {
            string queue = $"MQ{DateTime.Now.ToString("yyyyMMdd")}.TaskQueue";
            IAsyncMicroService Receiver = new SentEmailService(queue, 60, 10, false, null, 2, 10);
            Receiver.Init();
            Receiver.StartAsync();

            return Json("Success, StartAsync()", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 雙向服務，寫入資料後，又取得資料
        /// 需搭配Consumer()，Consumer需先點開，讓另一個執行序去跑
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {            
            
            //producer
            IMQProducerFacade MQAdapter = new MQProducerFacade();
            if (!MQAdapter.IsConnected())
            {
                MQAdapter.Init();
                MQAdapter.Connect();
            }
            string message = SetMessage();
            string queueName = $"MQ{DateTime.Now.ToString("yyyyMMdd")}.TaskQueue";
            MQAdapter.Publish(queueName, message);

            string replyQueue = $"MQ{DateTime.Now.ToString("yyyyMMdd")}.ReplyMessage";
            ViewBag.Message =  MQAdapter.GetReturnMessage(replyQueue);

            return View();
        }

        /// <summary>
        /// 單向寫入資料
        /// </summary>
        /// <returns></returns>
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
           
            var MQAdapter = new MQProducerFacade();
            if (!MQAdapter.IsConnected())
            {
                MQAdapter.Init();
                MQAdapter.Connect();
            }
            string message = SetMessage();
            string queueName = $"MQ{DateTime.Now.ToString("yyyyMMdd")}.TaskQueue";
            MQAdapter.Publish(queueName, message);
            return View();
        }
        private string SetMessage()
        {
            ScheduleTask taskData = new ScheduleTask
            {
                ID = 1,
                Status = ScheduleStatus.Initialized,
                ScheduleType = ScheduleType.PushMessage,
                Name = "dont know what this for",
                ScheduleData = $"This is the Producer message {DateTime.Now.ToLongTimeString()}",
                Description = "dont know what this for",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                SubmittedDate = DateTime.Now

            };

            return JsonConvert.SerializeObject(taskData);
        }
    }
}