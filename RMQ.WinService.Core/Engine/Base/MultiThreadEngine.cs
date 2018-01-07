
using RMQ.Utility.Nlog;
using RMQ.WinService.Core.Engine.Module;
using RMQ.WinService.Core.Schedule.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;


namespace RMQ.WinService.Core.Engine.Base
{
    internal abstract class MultiThreadEngine
    {
        //protected int _Counter = 0;
        protected AutoResetEvent MaxThreadAutoReset = new AutoResetEvent(false);
        protected object _Lock = new object();
        protected int _MaxThread = 10;
        protected int _IntervalSec = 1;
        protected bool _Stopping = false;
        protected string _ScheduleServer;

        protected List<ScheduleBase> _Schedules = new List<ScheduleBase>();
        /// <summary>
        /// 抽象化取得清單表下個訊息
        /// </summary>
        /// <returns></returns>
        protected abstract ScheduleBase GetNextJob();
        
        public MultiThreadEngine(string scheduleServer, int maxThread, int intervalSec)
        {
            _ScheduleServer = scheduleServer;
            _MaxThread = maxThread;
            _IntervalSec = intervalSec;
            ThreadCounter = 0;
        }

        public int ThreadCounter { get; set; }
        
        /// <summary>
        /// 主執行序引擎開始
        /// </summary>
        public void Start()
        {
            _Stopping = false;
            Thread mainSchedule = new Thread(MainScheduleThread);
            mainSchedule.Start();
        }
        /// <summary>
        /// 主執行序引擎結束
        /// </summary>
        public void Stop()
        {
            _Stopping = true;
            foreach (ScheduleBase schedule in _Schedules)
            {
                if (schedule != null) schedule.Stop();
            }
        }
        /// <summary>
        /// 主要服務程序，該服務會以設定的maxThread為上限，建構執行程序，到執行上續上限為止，或是服務訊息已消耗完畢，無訊息則Sleep
        /// </summary>
        private void MainScheduleThread()
        {
            while (true)
            {
                try
                {
                    if (ThreadCounter < _MaxThread)
                    {
                        //Option2:
                        CallScheduelBaseII();
                    }
                    else
                    {
                        MaxThreadAutoReset.WaitOne();
                    }

                    if (_Stopping)
                    {
                        while (true)
                        {
                            if (ThreadCounter == 0) return;
                            MaxThreadAutoReset.WaitOne();
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogService.Instance.Error($"{DateTime.Now} Error: MainScheduleThread()休息五秒。Message: {ex.Message}。StackTrace= {ex.StackTrace}");
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }

        /// <summary>
        /// 執行清單
        /// </summary>
        private void CallScheduelBaseII()
        {
            ScheduleBase schedule = GetNextJob();//去RabbitMQ 取下一個Message
            if (schedule != null)
            {
                //紀錄開始動作
                ScheduleTaskModule.Start(schedule.Task.ID, string.Format("{0} Start, Run Server:{1}", schedule.Task.ScheduleType.ToString(), _ScheduleServer));
                lock (_Lock)
                {
                    _Schedules.Add(schedule);
                    schedule._OnFinished += new ScheduleBase.FinishedHandler(ScheduleFinish);//實作Schedule  Finish
                                        
                    
                    ThreadCounter++;
                    if (Debugger.IsAttached)
                        schedule.GO();//測試用
                    else
                    {
                        Thread thread = new Thread(schedule.GO);
                        thread.Start();
                    }

                }
            }
            else
            {
                ScheduleTaskModule.NoMessage(_IntervalSec);
                System.Threading.Thread.Sleep(_IntervalSec * 1000);
            }
        }
        /// <summary>
        /// 服務清單結束後
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="id"></param>
        /// <param name="success"></param>
        /// <param name="message"></param>
        /// <param name="workSeconds"></param>
        protected void ScheduleFinish(ScheduleBase schedule, int id, bool success, string message, double workSeconds)
        {
            try
            {
                lock (_Lock)
                {
                    ThreadCounter--;
                    _Schedules.Remove(schedule);
                    MaxThreadAutoReset.Set();
                }

                if (success)
                {
                    //回壓狀態，之後寫入至ELK
                    ScheduleTaskModule.Complete(id, string.Format("{0} Completed,Use Time(sec):{1}, Run Server:{2}", schedule.Task.ScheduleType.ToString(), workSeconds, _ScheduleServer));
                }
                else
                {
                    //回壓狀態，之後寫入至ELK
                    ScheduleTaskModule.Fail(id, string.Format("{0} Fail,Error Message:{1}, Run Server:{2}", schedule.Task.ScheduleType.ToString(), message, _ScheduleServer));
                }
            }
            catch (Exception ex)
            {
                ScheduleTaskModule.Exception(id, $"{schedule.Task.ScheduleType.ToString()} Exception,Error Message:{message}, Run Server:{_ScheduleServer}, ExceptionMsg:{ex.Message}, ExceptionStackTrace:{ex.StackTrace}");
            }
        }






    }
}
