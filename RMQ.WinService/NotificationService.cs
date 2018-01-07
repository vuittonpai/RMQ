using RMQ.WinService.Core.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RMQ.WinService
{
    partial class NotificationService : ServiceBase
    {

        PushMessageService pushMessageService;
        /// <summary>
        /// 推波服務建構式
        /// </summary>
        public NotificationService()
        {
            pushMessageService = new PushMessageService();
            InitializeComponent();
        }

        /// <summary>
        /// 服務開始
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            int maxThread = 1;
            int intervalSec = 30;//如果Queue無資料，會停30秒

            pushMessageService.Start(Environment.MachineName, maxThread, intervalSec);
        }
        /// <summary>
        /// 服務結束
        /// </summary>
        protected override void OnStop()
        {
            pushMessageService.Stop();
        }
    }
}
