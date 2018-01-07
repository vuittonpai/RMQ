using RMQ.Core.EventArg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMQ.Core.MicroService
{
    /// <summary>
    /// 此範例是設計在讓服務之間可以溝通的模式
    /// 一種背景處理模式
    /// </summary>
    public interface IAsyncMicroService
    {
        /// <summary>
        /// 連線
        /// </summary>
        void Connect();
        /// <summary>
        /// 啟用服務之間溝通
        /// </summary>
        void StartAsync();
        /// <summary>
        /// 接收回傳方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnMessageReceived(object sender, MessageReceivedEventArgs e);
    }
}
