using Newtonsoft.Json;
using RMQ.Core.DTO;
using RMQ.Core.MicroService;
using RMQ.Core.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RMQ.WebApi.Controllers
{
    public class ValuesController : ApiController
    {
        [Route("Produce")]
        [HttpGet]
        public string Produce()
        {
            IMQProducerFacade MQAdapter = new MQProducerFacade();
            if (!MQAdapter.IsConnected())
            {
                //MQAdapter.Init();
                MQAdapter.Connect();
            }
            string message = SetMessage();
            string queueName = $"MQ{DateTime.Now.ToString("yyyyMMdd")}.TaskQueue";
            MQAdapter.Publish(queueName, message);

            return "Sent Success";
      
        }

        // GET api/values
        public IEnumerable<string> Get()
        {
            string queue = $"MQ{DateTime.Now.ToString("yyyyMMdd")}.TaskQueue";
            IAsyncMicroService Receiver = new SentEmailService(queue, 60, 10, false, null, 2, 10);
            //Receiver.Init();
            Receiver.StartAsync();

            return new string[] { "Success", "StartAsync()" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
            
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

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
