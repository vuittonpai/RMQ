

namespace RMQ.WinService.Core.Engine
{
    
    public class PushMessageService
    {
        PushMessageEngine pushMessageEngine = null;
        /// <summary>
        /// 推波服務開始
        /// </summary>
        /// <param name="scheduleServer"></param>
        /// <param name="maxThread"></param>
        /// <param name="intervalSec"></param>
        public void Start(string scheduleServer, int maxThread, int intervalSec)
        {
            pushMessageEngine = new PushMessageEngine(scheduleServer, maxThread, intervalSec);
            pushMessageEngine.Start();
        }
        /// <summary>
        /// 推波服務結束
        /// </summary>
        public void Stop()
        {
            if (pushMessageEngine != null) pushMessageEngine.Stop();
        }

    }
}
