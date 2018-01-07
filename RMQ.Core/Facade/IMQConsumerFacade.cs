using RMQ.Core.EventArg;
using System;

namespace RMQ.Core.Facade
{
    public interface IMQConsumerFacade<T>
    {
        /// <summary>
        /// 是否連線
        /// </summary>
        /// <returns></returns>
        bool IsConnected();
        /// <summary>
        /// 建立連線
        /// </summary>
        void Connect();
        /// <summary>
        /// 啟動Consumer
        /// </summary>
        void Comsume();
        /// <summary>
        /// Producer端發送訊息
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="message"></param>
        void Publish(string queueName, string message);
        /// <summary>
        /// 啟用服務之間溝通
        /// </summary>
        void StartAsync(T microService);
        /// <summary>
        /// 單一程序取得Q訊息用
        /// </summary>
        /// <returns></returns>
        string StartDequeue();
        /// <summary>
        /// 關閉連線
        /// </summary>
        void Disconnect();
        /// <summary>
        /// 觸發機制
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceivedII;
        /// <summary>
        /// 將訊息往前丟，搭配封裝內容模式
        /// </summary>
        void OnMessageReceived(object sender, MessageReceivedEventArgs e);
    }
}
