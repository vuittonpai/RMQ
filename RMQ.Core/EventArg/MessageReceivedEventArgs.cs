using RabbitMQ.Client.Events;
using System;


namespace RMQ.Core.EventArg
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string Message { get; set; }
        public BasicDeliverEventArgs EventArgs { get; set; }
        public Exception Exception { get; set; }
        public string result { get; set; }
    }
}
