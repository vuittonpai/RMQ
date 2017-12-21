using RMQ.Core.EventArg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMQ.Core.MicroService
{
    public interface IAsyncMicroService
    {
        void Init(string ip, int port, string userName, string password, ushort heartbeat);
        void StartAsync();
        void OnMessageReceived(object sender, MessageReceivedEventArgs e);
    }
}
