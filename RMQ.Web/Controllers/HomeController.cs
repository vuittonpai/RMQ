﻿using Newtonsoft.Json;
using RMQ.Core.MicroService;
using RMQ.Core.Producer;
using RMQ.DataClass.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            var Receiver = new PushNotificationService("MQ20171206.TaskQueue", 1000, 10, false, null, 2, 10);
            Receiver.Init("localhost", 5672, "guest", "guest", 30);
            Receiver.Start();
            ViewBag.Message = "";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            //修改方法二: 多一層介面
            
            var MQAdapter = new MessageQueueProducerAdapter();
            if (!MQAdapter.IsConnected())
            {
                MQAdapter.Init("127.0.0.1", 5672, "guest", "guest", 30);
                MQAdapter.Connect();
            }
            string message = SetMessage();//string.Format("This is the Producer message {0}", DateTime.Now.ToLongTimeString());
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