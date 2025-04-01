using Confluent.Kafka;
using DTO.InsuranceIncidents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataReader
{
    public class KafkaConsumer
    {
        private readonly string _brokerList;
        private readonly string _topic;
        private readonly string _groupId;

        public KafkaConsumer(string brokerList, string topic, string groupId)
        {
            _brokerList = brokerList;
            _topic = topic;
            _groupId = groupId;
        }

        public async Task ConsumeMessages(CancellationToken cancellationToken, string messageType)
        {
            var messageProcessor = ProcessorFactory.CreateProcessor(messageType);
            var config = new ConsumerConfig
            {
                BootstrapServers = _brokerList,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, byte[]>(config).Build();
            consumer.Subscribe(_topic);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(cancellationToken);
                        Console.WriteLine($"Message received from {consumeResult.TopicPartitionOffset}");
                        await messageProcessor.ProcessAsync(consumeResult.Message.Value, cancellationToken);                        
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Consume error: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Ensure the consumer leaves the group cleanly and final offsets are committed.
                consumer.Close();
            }
        }
    }
}