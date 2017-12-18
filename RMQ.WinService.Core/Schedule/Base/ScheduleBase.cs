using RMQ.DataClass;
using RMQ.DataClass.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMQ.WinService.Core.Schedule.Base
{
    internal abstract class ScheduleBase
    {
        public delegate void FinishedHandler(ScheduleBase schedule, int index, bool success, string message, double thisSeconds);
        public event FinishedHandler _OnFinished;
        private int _Id = 0;
        protected string _Message = string.Empty;
        protected bool _Stopping = false;

        public ScheduleTask Task { get; set; }

        public ScheduleBase(int id)
        {
            _Id = id;
        }
        public void Stop()
        {
            _Stopping = true;
        }

        /// <summary>
        /// Schedule 啟動
        /// </summary>
        public void GO()
        {
            double workSeconds = 0;
            bool workResult = false;
            try
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                workResult = Work();

                sw.Stop();
                workSeconds = sw.Elapsed.TotalSeconds;
            }
            catch (Exception ex)
            {
                _Message = ex.Message;
            }

            if (_OnFinished != null)
                _OnFinished(this, _Id, workResult, _Message, workSeconds);
        }

        protected abstract bool Work();
    }
}
