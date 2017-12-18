using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMQ.Utility.Nlog
{
    /// <summary>
    /// 系統記錄的結構
    /// </summary>
    public struct LoggingKey
    {
        /// <summary>
        /// 處理系統記錄
        /// </summary>
        public const string SYSTEMLOG_CATEGORY = "SystemLog";
        /// <summary>
        /// 主機網路資訊記錄器
        /// </summary>
        public const string HOST_LOGGER_CATEGORY = "HostLogger";
        /// <summary>
        /// 一般資訊記錄器
        /// </summary>
        public const string NORMAL_LOGGER_CATEGORY = "NormalLogger";
        /// <summary>
        /// 附加的記錄屬性名稱
        /// </summary>
        public const string EXTEND_PROPERTIES_LOG_PROPERTY_NAME = "ExtendProperties";
    }
}
