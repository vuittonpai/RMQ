using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using RMQ.Utility.Nlog;

namespace RMQ.Adapter.Producer
{
    public class RMQAdapter : AMQPAdapter
    {
        private static readonly RMQAdapter _instance = new RMQAdapter();
        private IConnection _connection;

        static RMQAdapter(){}
        public static RMQAdapter Instance { get { return _instance; } }

        private NLogService logger = new NLogService("RMQ.Adapter.RMQAdapter");

        public override bool IsConnected => (_connection!=null && _connection.IsOpen);
        
        public override void Connect()
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
            //connectionFactory.AutomaticRecoveryEnabled = true;
            //connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(20);

            _connection = connectionFactory.CreateConnection();
            _connection.CallbackException += Connection_CallbackException;
            _connection.ConnectionShutdown += Connection_ConnectionShutdown;
        }

        public override void Disconnect()
        {
            if (_connection != null) _connection.Dispose();
        }

        public override object GetConnection() => _connection;
       
        public override AMQPAdapter Init(string hostName, int port, string userName, string password, ushort heartbeat)
        {
            this.hostName = hostName;
            this.port = port;
            this.userName = userName;
            this.password = password;
            this.heartbeat = heartbeat;
            return this;
        }

        public void PublishMessage(string queueName, string message, bool createQueue = true, IBasicProperties messageProperties = null, IDictionary<string, object> queueArgs = null)
        {
            this.Publish(queueName, message);
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
            logger.Info($"{DateTime.Now} Info: Connection_ConnectionShutdown。 connection= {connection} Message: {reason.Cause}");
            Console.WriteLine("connection_ConnectionShutdown " + reason.ToString());
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
