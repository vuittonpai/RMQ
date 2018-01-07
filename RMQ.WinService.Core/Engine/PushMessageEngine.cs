using RMQ.Core.DTO;
using RMQ.WinService.Core.Engine.Base;
using RMQ.WinService.Core.Engine.Module;
using RMQ.WinService.Core.Schedule;
using RMQ.WinService.Core.Schedule.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMQ.WinService.Core.Engine
{
    internal class PushMessageEngine : MultiThreadEngine
    {
        private bool enabled;
        /// <summary>
        /// 推波服務引擎建構式
        /// </summary>
        /// <param name="scheduleServer"></param>
        /// <param name="maxThread"></param>
        /// <param name="intervalSec"></param>
        public PushMessageEngine(string scheduleServer, int maxThread, int intervalSec) :base(scheduleServer, maxThread, intervalSec)
        {
            enabled = true;
        }
        /// <summary>
        /// 實作取得下一個推波訊息
        /// </summary>
        /// <returns></returns>
        protected override ScheduleBase GetNextJob()
        {
            if (!enabled) return null;

            PushMessageSchedule schedule = null;

            ScheduleTask scheduleTask = ScheduleTaskModule.GetNextScheduleTask(ScheduleType.PushMessage);


            if (scheduleTask != null)
            {
                schedule = new PushMessageSchedule(scheduleTask.ID);
                schedule.Task = scheduleTask;
            }

            return schedule;
        }
    }
}
