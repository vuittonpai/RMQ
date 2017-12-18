using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMQ.WinService.Core.Engine
{
    
    public class PushMessageService
    {
        PushMessageEngine pushMessageEngine = null;

        public void Start(string scheduleServer, int maxThread, int intervalSec)
        {
            pushMessageEngine = new PushMessageEngine(scheduleServer, maxThread, intervalSec);
            pushMessageEngine.Start();
        }

        public void Stop()
        {
            if (pushMessageEngine != null) pushMessageEngine.Stop();
        }

    }
}
