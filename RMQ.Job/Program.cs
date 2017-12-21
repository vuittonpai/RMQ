
using RMQ.Core.MicroService;
using System;


namespace RMQ.Job
{
    class Program :IDisposable
    {
        static IMicroService Receiver;
        static void Main(string[] args)
        {
            string queue = $"MQ{DateTime.Now.ToString("yyyyMMdd")}.TaskQueue";
            Receiver = new PushNotificationService(queue, 60, 10, false, null, 2, 10);
            Receiver.Init("localhost", 5672, "guest", "guest", 30);
            Receiver.Start();
            Receiver.Shutdown();//跑Job有個問題是，他不會斷連線。需要而外處理或是讓他打keyword跳出，目前只是demo用，所以先不管。好像又可以。奇怪
        }

        public void Dispose()
        {
            Receiver.Shutdown();
        }
    }
}
