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

        public override bool IsConnected => (_connection!=null && _connection.IsOpen);
        
        //public override void Connect()
        //{
        //    var connectionFactory = new ConnectionFactory
        //    {
        //        HostName = hostName,
        //        Port = port,
        //        UserName = userName,
        //        Password = password,
        //        RequestedHeartbeat = heartbeat //seconds
                
        //    };
        //    //回復連線機制: Connection Recovery
        //    connectionFactory.AutomaticRecoveryEnabled = true;
        //    connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(20);

        //    _connection = connectionFactory.CreateConnection();
        //    _connection.CallbackException += Connection_CallbackException;
        //    _connection.ConnectionShutdown += Connection_ConnectionShutdown;
        //}

        public override void Connect()
        {
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
                RequestedHeartbeat = 60,//heartbeat, //seconds
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromMilliseconds(100), //斷線後，每100毫秒重新連接

                //HostName = hostName,
                //Port = port

            };
            var process = Process.GetCurrentProcess();
            string rmqClientName = string.Format("{0}#{1}", process.ProcessName, process.Id);

            var ers = connectionFactory.EndpointResolverFactory(amqpEndpoints);
           

            //回復連線機制: Connection Recovery
            connectionFactory.AutomaticRecoveryEnabled = true;
            connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(20);//斷線後，每20秒重新連接

            _connection = connectionFactory.CreateConnection(ers, rmqClientName);
            //_connection = connectionFactory.CreateConnection();
            _connection.CallbackException += Connection_CallbackException;
            _connection.ConnectionShutdown += Connection_ConnectionShutdown;
        }


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
                        logger.Info($"{DateTime.Now} {channel.ChannelNumber} Info: 接收訊息。 QueueName= {queueName} Message: {returnMessage}");
                        channel.BasicAck(result.DeliveryTag, false);
                        logger.Info($"{DateTime.Now} {channel.ChannelNumber} Info: 回應收到。 QueueName= {queueName} Message: {returnMessage}");
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
                logger.Error($"{DateTime.Now}Error: TimeoutException={ e.Message} StackTrace: {e.StackTrace}");
                Console.WriteLine(e);
                return "";
            }
        }


        public override void Disconnect()
        {
            if (_connection != null) _connection.Dispose();
        }

        public override object GetConnection() => _connection;

        public override AMQPAdapter Init()
        {
            this.hostName = AppSettingConfig.getAppSettings("hostName");
            this.port = AppSettingConfig.getAppSettings("port", "5672");
            this.userName = AppSettingConfig.getAppSettings("userName");
            this.password = AppSettingConfig.getAppSettings("password");
            this.heartbeat = AppSettingConfig.getAppSettingsUshort("heartbeat", "30");
            return this;
        }

        public override AMQPAdapter Init(string hostName, int port, string userName, string password, ushort heartbeat)
        {
            this.hostName = hostName;
            this.port = port;
            this.userName = userName;
            this.password = password;
            this.heartbeat = heartbeat;
            return this;
        }


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

                    logger.Info($"{DateTime.Now} Info: 成功傳送訊息。 QueueName= {queueName} Message: {message}");
                }
            }
            catch(TimeoutException timeout)
            {
                logger.Error($"{DateTime.Now} Error: TimeoutException={ timeout.Message} StackTrace: {timeout.StackTrace}");
                this.Disconnect();
                Thread.Sleep(3000);
                Restart();
            }
            catch (Exception ex)
            {
                logger.Error($"{DateTime.Now}Error: TimeoutException={ ex.Message} StackTrace: {ex.StackTrace}");
                this.Disconnect();
            }
            
        }
        
        private void Connection_CallbackException(object sender, CallbackExceptionEventArgs e)
        {
            logger.Error($"{DateTime.Now} Error: connection_CallbackException()。 sender= {sender} Message: {e.Exception.Message}");
            Console.WriteLine("connection_CallbackException " + e.Exception.StackTrace);
        }
        private void Connection_ConnectionShutdown(object connection, ShutdownEventArgs reason)
        {
            //logger.Info($"{DateTime.Now} Info: Connection_ConnectionShutdown。 connection= {connection} Message: {reason.Cause}");
            //Console.WriteLine("connection_ConnectionShutdown " + reason.ToString());
            //ReStart();
        }
        private void Channel_ModelShutdown(object model, ShutdownEventArgs reason)
        {
            //logger.Info($"{DateTime.Now} Info: CHANNEL__MODEL_SHUTDOWN。 model= {model} Message: {reason.Cause}");
            Console.WriteLine("CHANNEL__MODEL_SHUTDOWN " + reason.ToString());
        }

        private void Restart()
        {
            if (!this.IsConnected)
            {
                this.Connect();
            }
        }

    }

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
