using Newtonsoft.Json;
using RMQ.Core.DTO;
using RMQ.Core.Producer;
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

        public ActionResult About()
        {            
            ViewBag.Message = "";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
           
            var MQAdapter = new MQProducerFacade();
            if (!MQAdapter.IsConnected())
            {
                MQAdapter.Init("127.0.0.1", 5672, "guest", "guest", 30);
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