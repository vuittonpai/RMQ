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
        public NotificationService()
        {
            pushMessageService = new PushMessageService();
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            int maxThread = 1;
            int intervalSec = 30;//如果Queue無資料，會停30秒

            pushMessageService.Start(Environment.MachineName, maxThread, intervalSec);
        }

        protected override void OnStop()
        {
            pushMessageService.Stop();
        }
    }
}
