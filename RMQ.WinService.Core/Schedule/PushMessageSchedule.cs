
using RMQ.WinService.Core.Schedule.Base;
using System;

namespace RMQ.WinService.Core.Schedule
{
    internal class PushMessageSchedule : ScheduleBase
    {
        public PushMessageSchedule(int id) : base( id)
        {
                
        }


        /// <summary>
        /// 實作執行 商業邏輯
        /// </summary>
        /// <returns></returns>
        protected override bool Work()
        {
            
            Console.WriteLine("商業邏輯: " + Task.ScheduleData);

            return (!string.IsNullOrEmpty(Task.ScheduleData));

        }
    }
}
