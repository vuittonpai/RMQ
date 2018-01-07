

namespace RMQ.Core.Facade
{
    public interface IMQProducerFacade
    {
        /// <summary>
        /// 確認是否連線
        /// </summary>
        /// <returns></returns>
        bool IsConnected();
        /// <summary>
        /// 建構連線
        /// </summary>
        void Connect();
        /// <summary>
        /// Producer端發送訊息
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="message"></param>
        void Publish(string queueName, string message);
        /// <summary>
        /// 關閉連線
        /// </summary>
        void Disconnect();
        /// <summary>
        /// 取得Queue裡面下一個message
        /// </summary>
        /// <param name="replyQueue"></param>
        /// <returns></returns>
        string GetReturnMessage(string replyQueue);

    }
}
