using RMQ.Core.MicroService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMQ.Job
{
    class Program
    {
        static void Main(string[] args)
        {
            string queue = $"MQ{DateTime.Now.ToString("yyyyMMdd")}.TaskQueue";
            IMicroService Receiver = new PushNotificationService(queue, 60, 10, false, null, 2, 10);
            Receiver.Init("localhost", 5672, "guest", "guest", 30);
            Receiver.Start();
            Receiver.Shutdown();
        }
    }
}
