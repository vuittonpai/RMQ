


using RMQ.Adapter.EventArg;

namespace RMQ.Core.MicroService
{
    public interface IMicroService
    {
        void Init(string ip, int port, string userName, string password, ushort heartbeat);
        void OnMessageReceived(object sender, MessageReceivedEventArgs e);
        void Shutdown();
        void Start();
        string StartDequeue();
    }
}
