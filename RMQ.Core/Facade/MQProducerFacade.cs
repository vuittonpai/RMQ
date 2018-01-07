using System;
using RMQ.Core.EventArg;
using RMQ.Core.Adapter;

namespace RMQ.Core.Facade
{
    /// <summary>
    /// 這個是給Producer用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MQProducerFacade : IMQProducerFacade
    {
        internal RMQAdapter Adapter { get; set; }
        public MQProducerFacade()
        {
            Adapter = RMQAdapter.Instance;
        }
        /// <summary>
        /// 建構連線
        /// </summary>
        public void Connect()
        {
            Adapter.Connect();
        }
        
        /// <summary>
        /// 確認是否連線
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return Adapter.IsConnected;
        }
        /// <summary>
        ///  Producer端發送訊息
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="message"></param>
        public void Publish(string queueName, string message)
        {
            Adapter.Publish(queueName, message);
        }
        /// <summary>
        /// 關閉連線
        /// </summary>
        public void Disconnect()
        {
            Adapter.Disconnect();
        }
        /// <summary>
        /// 取得Queue裡面下一個message
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public string GetReturnMessage(string queueName)
        {
            return Adapter.GetReturnMessage(queueName);
        }
        
    }
}
