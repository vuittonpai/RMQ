using RMQ.Core.EventArg;

namespace RMQ.Core.MicroService
{
    public interface IMicroService
    {
        /// <summary>
        /// 建構連線
        /// </summary>
        void Connect();
        /// <summary>
        /// 實作回傳方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnMessageReceived(object sender, MessageReceivedEventArgs e);
        /// <summary>
        /// 結束連線  
        /// </summary>
        void Shutdown();
        /// <summary>
        /// 啟用Consumer
        /// </summary>
        void Start();
    }
}
