
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RMQ.Core.Adapter;
using RMQ.Core.EventArg;
using RMQ.Utility.Nlog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RMQ.Core.Consumer
{
    public abstract class RMQConsumer : AMQPComsumer
    {
        protected RMQConsumer(string queueName, int timeout, ushort prefetchCount = 1, bool noAck = false
            , IDictionary<string, object> queueArgs = null, int ConsumerNumber = 2, int MessageNumber = 10)
        : base(queueName, timeout, prefetchCount, noAck, queueArgs)
        { }

        private NLogService logger = new NLogService("RMQ.Adapter.RMQConsumer");
        public event EventHandler<MessageReceivedEventArgs> _MessageReceived;
        //觸發機制，讓前面去實作商業邏輯
        protected void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            _MessageReceived?.Invoke(this, e);
        }

        internal override void StartAsync(AMQPAdapter amqpAdapter)
        {
            try
            {
                var connection = (IConnection)amqpAdapter.GetConnection();

                using (var channel = connection.CreateModel())
                {
                    channel.ModelShutdown += Channel_ModelShutdown;

                    while (!stopConsuming)
                    {
                        if (channel.ConsumerCount(queueName) < 2 || channel.MessageCount(queueName) > 10)
                        {
                            channel.QueueDeclare(queueName, true, false, false, queueArgs);
                            channel.BasicQos(0, prefetchCount, false);

                            var consumer = new EventingBasicConsumer(channel);
                            consumer.Received += OnConsumer_ReceivedIIIAsync;
                            channel.BasicConsume(queueName, noAck, consumer);
                        }
                        else
                        {
                            //Thread.Sleep(1000);
                        }
                    }
                }
            }
            catch (TimeoutException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception exception)
            {
                OnMessageReceived(this, new MessageReceivedEventArgs
                {
                    Exception = exception
                });
            }
        }

        internal override void Start(AMQPAdapter amqpAdapter)
        {
            try
            {
                var connection = (IConnection)amqpAdapter.GetConnection();

                using (var channel = connection.CreateModel())
                {
                    channel.ModelShutdown += Channel_ModelShutdown;

                    while (!stopConsuming)
                    {
                        if (channel.ConsumerCount(queueName) < 2 || channel.MessageCount(queueName) > 10)
                        {
                            //當Consumer少於2台，或是，Message大於10個
                            channel.QueueDeclare(queueName, true, false, false, queueArgs);
                            channel.BasicQos(0, prefetchCount, false);

                            var consumer = new EventingBasicConsumer(channel);
                            consumer.Received += OnConsumer_ReceivedIIAAA;
                            var test = channel.BasicConsume(queueName, noAck, consumer);
                            logger.Info($"{DateTime.Now} Info: Consumer啟動。channel: {channel.ChannelNumber}。QueueName= {queueName}。Message: {returnMessage}");
                        }
                        else
                        {
                            Thread.Sleep(3000);
                        }
                       
                    }

                }
            }
            catch (TimeoutException e)
            {
                Console.WriteLine(e);
                stopConsuming = true;
                
            }
            catch (Exception exception)
            {
                OnMessageReceived(this, new MessageReceivedEventArgs
                {
                    Exception = exception
                });
                stopConsuming = true;
            }
        }

        private string returnMessage = "";
        internal override string StartDequeue(AMQPAdapter amqpAdapter)
        {
            try
            {
                var connection = (IConnection)amqpAdapter.GetConnection();
  
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queueName, true, false, false, null);
                    channel.BasicQos(0, 1, false);
                    
                    //var consumer = new QueueingBasicConsumer(channel);
                    //channel.BasicConsume(queueName, false, consumer);
                    //var messageIsAvailable = consumer.Queue.Dequeue(1 * 1000, out args);

                    var result = channel.BasicGet(queueName, false);
                    

                    if (result != null)
                    {
                        //returnMessage = Encoding.UTF8.GetString(args.Body);
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
                stopConsuming = true;
                return returnMessage;
            }
            catch (Exception e)
            {
                logger.Error($"{DateTime.Now}Error: TimeoutException={ e.Message} StackTrace: {e.StackTrace}");
                Console.WriteLine(e);
                //OnMessageReceived(new MessageReceivedEventArgs
                //{
                //    Exception = e
                //});
                stopConsuming = true;
                return returnMessage;
            }
        }
       
        //// 方法二: 使用EventHandler 取接，
        protected void OnConsumer_ReceivedIIAAA(object sender, BasicDeliverEventArgs e)
        {
            
            try
            {
                var consumer = sender as EventingBasicConsumer;
                var message = Encoding.UTF8.GetString(e.Body);
                logger.Info($"{DateTime.Now} Info: 回應收到。ConsumerTag: {consumer.ConsumerTag}。QueueName= {queueName}。Message: {returnMessage}");
                _MessageReceived?.Invoke(this, new MessageReceivedEventArgs {
                    Message = message,
                    EventArgs = e
                });
                AcknowledgeMessage(e.DeliveryTag, consumer.Model);
                    
            }
            catch (Exception exception)
            {
                stopConsuming = true;
                Thread.CurrentThread.Abort();
                throw exception;
            }
            
        }

        protected void OnConsumer_ReceivedIIIAsync(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                var consumer = sender as EventingBasicConsumer;
                var message = Encoding.UTF8.GetString(e.Body);
                logger.Info($"{DateTime.Now} Info: 回應收到。ConsumerTag: {consumer.ConsumerTag}。QueueName= {queueName}。Message: {returnMessage}");
                AcknowledgeMessage(e.DeliveryTag, consumer.Model);
            }
            catch (Exception exception)
            {
                stopConsuming = true;
                Thread.CurrentThread.Abort();
                throw exception;
            }

        }

        public void AcknowledgeMessage(ulong deliveryTag, IModel channel)
        {
            logger.Info($"{DateTime.Now} Info: 回應收到。DeliveryTag: {deliveryTag}。channel: {channel.ChannelNumber}。QueueName= {queueName}。Message: {returnMessage}");
            channel.BasicAck(deliveryTag, false);
        }


        private void HandleBasicCancelOk(string consumerTag)
        {

        }

        private void Channel_ModelShutdown(object model, ShutdownEventArgs reason)
        {
            //logger.Error($"{DateTime.Now} Error: CHANNEL__MODEL_SHUTDOWN()。 model= {model} Message: {reason.Cause}");
            Console.WriteLine("CHANNEL__MODEL_SHUTDOWN " + reason.ToString());
        }

    }
}
