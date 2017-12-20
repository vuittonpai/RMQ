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

        public PushMessageEngine(string scheduleServer, int maxThread, int intervalSec) :base(scheduleServer, maxThread, intervalSec)
        {
            enabled = true;
        }

        //protected override void StartReceiveQueue()
        //{
        //    PushNotificationService pushMessageSchedule = new PushNotificationService("MQ20171206.TaskQueue", 1000);
        //    pushMessageSchedule.Init();
        //    pushMessageSchedule.StartAsync();//即便使用不倫巡，還是沒辦法call OnFinished()，也無法取道message值
        //    ThreadCounter++;
        //    //OnFinished += new FinishedHandler(ScheduleFinish);
        //}


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
