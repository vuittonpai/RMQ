using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using RMQ.Utility.Nlog;
using System.Configuration;
using RMQ.Utility;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace RMQ.Core.Adapter
{
    internal class RMQAdapter : AMQPAdapter
    {
        private static readonly RMQAdapter _instance = new RMQAdapter();
        private IConnection _connection;

        public static RMQAdapter Instance { get { return _instance; } }

        private NLogService logger = new NLogService("RMQ.Adapter.RMQAdapter");
        /// <summary>
        /// 是否連線
        /// </summary>
        public override bool IsConnected => (_connection!=null && _connection.IsOpen);
        /// <summary>
        /// 預留連線
        /// </summary>
        private void ConnectDirectly()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = hostName,
                Port = port,
                UserName = userName,
                Password = password,
                RequestedHeartbeat = heartbeat //seconds

            };
            //回復連線機制: Connection Recovery
            connectionFactory.AutomaticRecoveryEnabled = true;
            connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(20);

            _connection = connectionFactory.CreateConnection();
            _connection.CallbackException += Connection_CallbackException;
            _connection.ConnectionShutdown += Connection_ConnectionShutdown;
        }
        /// <summary>
        /// HA方式連線至多台Queue機器
        /// </summary>
        public override void Connect()
        {
            //取得正確連線格式
            Regex rxUri = new Regex(@"^amqp://(.*?):(.*?)@(.*)$", RegexOptions.Compiled);
            string uriSetting = ConfigurationManager.AppSettings["rmqUri"];

            var uriItems = rxUri.Match(uriSetting).Groups;
            if (uriItems.Count != 4)
                throw new Exception("rmqUri設定錯誤");

            List<AmqpTcpEndpoint> amqpEndpoints = new List<AmqpTcpEndpoint>();
            foreach (var host in uriItems[3].Value.Split(','))
            {
                var h = host.Split(':');
                amqpEndpoints.Add(new AmqpTcpEndpoint()
                {
                    HostName = h[0],
                    Port = Convert.ToInt32(h[1]),
                });
            }

            var connectionFactory = new ConnectionFactory
            {
                UserName = uriItems[1].Value,
                Password = uriItems[2].Value,
                RequestedHeartbeat = 60,//heartbeat by seconds
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromMilliseconds(100), //斷線後，每100毫秒重新連接
            };
            var process = Process.GetCurrentProcess();
            string rmqClientName = $"{process.ProcessName}#{ process.Id}";
            var endPoints = connectionFactory.EndpointResolverFactory(amqpEndpoints);
           

            //回復連線機制: Connection Recovery
            connectionFactory.AutomaticRecoveryEnabled = true;
            connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(20);//斷線後，每20秒重新連接

            _connection = connectionFactory.CreateConnection(endPoints, rmqClientName);
            _connection.CallbackException += Connection_CallbackException;
            _connection.ConnectionShutdown += Connection_ConnectionShutdown;
        }
        /// <summary>
        /// 直接取得Queue裡面下一個message
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        internal string GetReturnMessage(string queueName)
        {
            try
            {
                string returnMessage = string.Empty;
                var connection = (IConnection)this.GetConnection();

                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queueName, true, false, false, null);
                    channel.BasicQos(0, 1, false);
                    
                    var result = channel.BasicGet(queueName, false);
                    if (result != null)
                    {
                        IBasicProperties props = result.BasicProperties;
                        byte[] body = result.Body;
                        returnMessage = Encoding.UTF8.GetString(body);
                        channel.BasicAck(result.DeliveryTag, false);
                        NLogService.Instance.Info($"{DateTime.Now} {channel.ChannelNumber} Info: 取得訊息。 QueueName= {queueName} Message: {returnMessage}");
                    }
                }
                return returnMessage;
            }
            catch (TimeoutException e)
            {
                Console.WriteLine(e);
                return "";
            }
            catch (Exception e)
            {
                NLogService.Instance.Error($"{DateTime.Now}Error: TimeoutException={ e.Message} StackTrace: {e.StackTrace}");
                Console.WriteLine(e);
                return "";
            }
        }
        /// <summary>
        /// 關閉連線
        /// </summary>
        public override void Disconnect()
        {
            if (_connection != null) _connection.Dispose();
        }
        /// <summary>
        /// 取得連線物件
        /// </summary>
        /// <returns></returns>
        public override object GetConnection() => _connection;
        /// <summary>
        /// 目前預留，指定帳號
        /// </summary>
        /// <returns></returns>
        public override AMQPAdapter Init()
        {
            this.hostName = AppSettingConfig.getAppSettings("hostName");
            this.port = AppSettingConfig.getAppSettings("port", "5672");
            this.userName = AppSettingConfig.getAppSettings("userName");
            this.password = AppSettingConfig.getAppSettings("password");
            this.heartbeat = AppSettingConfig.getAppSettingsUshort("heartbeat", "30");
            this.ConnectDirectly();
            return this;
        }
        /// <summary>
        /// 目前預留，指定帳號
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="heartbeat"></param>
        /// <returns></returns>
        public override AMQPAdapter Init(string hostName, int port, string userName, string password, ushort heartbeat)
        {
            this.hostName = hostName;
            this.port = port;
            this.userName = userName;
            this.password = password;
            this.heartbeat = heartbeat;
            this.ConnectDirectly();
            return this;
        }
        /// <summary>
        /// Producer端發送訊息
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="message"></param>
        /// <param name="createQueue"></param>
        /// <param name="messageProperties"></param>
        /// <param name="queueArgs"></param>
        public override void Publish(string queueName, string message, bool createQueue = true, IBasicProperties messageProperties = null, IDictionary<string, object> queueArgs = null)
        {
            try
            {
                if (!IsConnected) Connect();
                using (var channel = _connection.CreateModel())
                {
                    channel.ModelShutdown += Channel_ModelShutdown;
                    if (createQueue) channel.QueueDeclare(queueName, true, false, false, queueArgs);
                    var payload = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(string.Empty, queueName,
                        messageProperties ?? RabbitMQProperties.CreateDefaultProperties(channel), payload);

                    NLogService.Instance.Info($"{DateTime.Now} Info: 成功傳送訊息。 QueueName= {queueName} Message: {message}");
                }
            }
            catch(TimeoutException timeout)
            {
                NLogService.Instance.Error($"{DateTime.Now} Error: TimeoutException={ timeout.Message} StackTrace: {timeout.StackTrace}");
                this.Disconnect();
                Thread.Sleep(3000);
                Restart();
            }
            catch (Exception ex)
            {
                NLogService.Instance.Error($"{DateTime.Now}Error: TimeoutException={ ex.Message} StackTrace: {ex.StackTrace}");
                this.Disconnect();
            }
            
        }
        /// <summary>
        /// 連線回傳錯誤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connection_CallbackException(object sender, CallbackExceptionEventArgs e)
        {
            NLogService.Instance.Error($"{DateTime.Now} Error: connection_CallbackException()。 sender= {sender} Message: {e.Exception.Message}");
            Console.WriteLine("connection_CallbackException " + e.Exception.StackTrace);
        }
        /// <summary>
        /// 連線中斷
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="reason"></param>
        private void Connection_ConnectionShutdown(object connection, ShutdownEventArgs reason)
        {
            //logger.Info($"{DateTime.Now} Info: Connection_ConnectionShutdown。 connection= {connection} Message: {reason.Cause}");
            //Console.WriteLine("connection_ConnectionShutdown " + reason.ToString());
            //ReStart();
        }
        /// <summary>
        /// Channel關閉
        /// </summary>
        /// <param name="model"></param>
        /// <param name="reason"></param>
        private void Channel_ModelShutdown(object model, ShutdownEventArgs reason)
        {
            //logger.Info($"{DateTime.Now} Info: CHANNEL__MODEL_SHUTDOWN。 model= {model} Message: {reason.Cause}");
            Console.WriteLine("CHANNEL__MODEL_SHUTDOWN " + reason.ToString());
        }
        /// <summary>
        /// 重新連線
        /// </summary>
        private void Restart()
        {
            if (!this.IsConnected)
            {
                this.Connect();
            }
        }

    }

    /// <summary>
    /// Producer傳送物件屬性
    /// </summary>
    public static class RabbitMQProperties
    {
        public static IBasicProperties CreateDefaultProperties(IModel model)
        {
            var properties = model.CreateBasicProperties();

            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.ContentEncoding = "UTF-8";

            return properties;
        }
    }
}
