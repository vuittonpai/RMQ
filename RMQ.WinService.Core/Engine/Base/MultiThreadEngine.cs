
using RMQ.WinService.Core.Engine.Module;
using RMQ.WinService.Core.Schedule.Base;
using System;
using System.Collections.Generic;
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

        protected abstract ScheduleBase GetNexJob();

        ///**************Option1: 需要的
        //protected abstract void StartReceiveQueue();
        //public delegate void FinishedHandler(int index, bool success, string message, double thisSeconds);
        //public event FinishedHandler OnFinished;
        ///**************Option1 End

        public MultiThreadEngine(string scheduleServer, int maxThread, int intervalSec)
        {
            _ScheduleServer = scheduleServer;
            _MaxThread = maxThread;
            _IntervalSec = intervalSec;
            ThreadCounter = 0;
        }

        public int ThreadCounter { get; set; }
        

        public void Start()
        {
            _Stopping = false;
            Thread mainSchedule = new Thread(MainScheduleThread);
            mainSchedule.Start();
        }
        public void Stop()
        {
            _Stopping = true;
            foreach (ScheduleBase schedule in _Schedules)
            {
                if (schedule != null) schedule.Stop();
            }
        }

        private void MainScheduleThread()
        {
            while (true)
            {
                try
                {
                    if (ThreadCounter < _MaxThread)
                    {
                        //Option1: 搭配我的RMQComsuer，A: 失敗，即便拿掉倫巡，也不知道thread結束於否。
                        //lock (_Lock)
                        //{
                        //    StartReceiveQueue();//去RabbitMQ 取下一個Message倫巡
                        //}
                        //Option2:
                        CallScheduelBase();

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
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }

        private void CallScheduelBase()
        {
            ScheduleBase schedule = GetNexJob();//去RabbitMQ 取下一個Message
            if (schedule != null)
            {
                //紀錄動作
                ScheduleTaskModule.Start(schedule.Task.ID, string.Format("{0} Start, Run Server:{1}", schedule.Task.ScheduleType.ToString(), _ScheduleServer));
                lock (_Lock)
                {
                    _Schedules.Add(schedule);
                    schedule._OnFinished += new ScheduleBase.FinishedHandler(ScheduleFinish);//實作Schedule  Finish
                                        
                    //Thread thread = new Thread(schedule.GO);
                    //thread.Start();
                    ThreadCounter++;
                    schedule.GO();//測試用
        
                }
            }
            else
            {
                System.Threading.Thread.Sleep(_IntervalSec * 1000);
            }
        }

        protected void ScheduleFinish(ScheduleBase schedule, int id, bool success, string message, double workSeconds)
        {
            try
            {
                lock (_Lock)
                {
                    ThreadCounter--;
                    //_Schedules.Remove(schedule);
                    MaxThreadAutoReset.Set();
                }

                if (success)
                {
                    //ScheduleTaskModule.Complete(id, string.Format("{0} Completed,Use Time(sec):{1}, Run Server:{2}", schedule.Task.ScheduleType.ToString(), workSeconds, _ScheduleServer));
                }
                else
                {
                    //ScheduleTaskModule.Fail(id, string.Format("{0} Fail,Error Message:{1}, Run Server:{2}", schedule.Task.ScheduleType.ToString(), message, _ScheduleServer));
                }
            }
            catch (Exception ex)
            {

            }
        }






    }
}
