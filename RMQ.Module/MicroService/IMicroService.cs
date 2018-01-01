


using RMQ.Core.EventArg;

namespace RMQ.Core.MicroService
{
    public interface IMicroService
    {
        void Init();
        void OnMessageReceived(object sender, MessageReceivedEventArgs e);
        void Shutdown();
        void Start();
    }
}
