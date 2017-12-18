using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RMQ.Utility.Nlog
{
    public class NLogService
    {
        private static Logger logger = null;

        private static Lazy<Logger> loggerStatic;

        /// <summary>
        /// 方法一建構式
        /// </summary>
        /// <param name="ruleName"></param>
        public NLogService(string ruleName)
        {
            logger = (string.IsNullOrEmpty(ruleName)) ? LogManager.GetCurrentClassLogger() : LogManager.GetLogger(ruleName);
        }

        public NLogService()
        {
            loggerStatic = new Lazy<Logger>(() => LogManager.GetCurrentClassLogger());
            if(loggerStatic.IsValueCreated){

                loggerStatic.Value.Info("延遲載入");
            }
        }

        /// <summary>
        /// 方式二在建構式
        /// </summary>
        public static NLogService Instance
        {
            get
            {
                if (Thread.GetData(Thread.GetNamedDataSlot(LoggingKey.SYSTEMLOG_CATEGORY)) == null)
                {
                    Thread.SetData(Thread.GetNamedDataSlot(LoggingKey.SYSTEMLOG_CATEGORY), new NLogService());
                }
                return Thread.GetData(Thread.GetNamedDataSlot(LoggingKey.SYSTEMLOG_CATEGORY)) as NLogService;
            }
        }

        public void Debug(object message)
        {
            logger.Debug(message);
        }

        public void Info(object message)
        {
            logger.Info(message);
        }

        public void InfoStatic(object message)
        {
            loggerStatic.Value.Info(message);
        }


        public void Warn(object message)
        {
            logger.Warn(message);
        }

        public void Error(object message)
        {
            logger.Error(message);
        }

        public void Fatal(object message)
        {
            logger.Fatal(message);
        }

        public void Trace(string message)
        {

            logger.Info(message);
        }

    }
}
