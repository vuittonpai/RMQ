using Newtonsoft.Json;
using RMQ.Core.DTO;
using RMQ.Core.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebApi.Attributes;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class PushNotificationController: ApiController
    {
        //[Authentication]
        [AllowAnonymous]
        [HttpPost]
        [Route("PushNotification")]
        public string PushNotification([FromBody] AccountArgs account)
        {
            IMQProducerFacade MQAdapter = new MQProducerFacade();
            if (!MQAdapter.IsConnected())
            {
                MQAdapter.Connect();
            }
            string queueName = $"MQ{DateTime.Now.ToString("yyyyMMdd")}.TaskQueue";
            if (!string.IsNullOrEmpty(account.MessageContent))
                MQAdapter.Publish(queueName, SetMessage(account.MessageContent));
            else
                return "Please Enter MessageContent";

            return "Sent Success";

        }
        private string SetMessage(string message)
        {
            ScheduleTask taskData = new ScheduleTask
            {
                ID = 1,
                Status = ScheduleStatus.Initialized,
                ScheduleType = ScheduleType.PushMessage,
                Name = "dont know what this for",
                ScheduleData = message,
                Description = "dont know what this for",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                SubmittedDate = DateTime.Now
            };

            return JsonConvert.SerializeObject(taskData);
        }

    }
}