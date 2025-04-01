using Confluent.Kafka;
using DTO.InsuranceIncidents;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace DataGenerator
{
    public class EventGenerator
    {
        ProducerConfig _producerConfig;
        public EventGenerator(string brokerList)
        {
            _producerConfig = new ProducerConfig { BootstrapServers = brokerList };
        }
        public async Task Generate(string type, int count, int delayMs)
        {
            using var producer = new ProducerBuilder<string, byte[]>(_producerConfig).Build();
            var topicName = "insurance-incidents-" + type;

            for (int i = 0; i < count; i++)
            {
                var message = GenerateEvent(type, i);
                SendMessage(Serialize(message), producer, topicName);

                await Task.Delay(delayMs);
            }
        }

        private IMessage GenerateEvent(string type, int number)
        {
            return true switch
            {
                true when typeof(CarIncident).Name == type => GetCarIncident(),
                true when typeof(FlatIncident).Name == type => GetFlatIncident(),
                true when typeof(HealthIncident).Name == type => GetHealthIncident(),
                _ => throw new ArgumentException("Unsupported model " + type)
            };
        }

        public byte[] Serialize(IMessage record)
        {
            using (var stream = new MemoryStream())
            {
                using (var codedStream = new CodedOutputStream(stream))
                {
                    record.WriteTo(codedStream);
                    codedStream.Flush();
                    return stream.ToArray();
                }
            }
        }

        private HealthIncident GetHealthIncident()
        {
            return new HealthIncident()
            {
                FirstName = "FirstName" + DateTime.UtcNow.Second,
                LastName = "LastName" + DateTime.UtcNow.Second,
                BirthDate = DateTime.UtcNow.AddYears(-Random.Shared.Next(18, 70)).ToTimestamp(),
            };
        }

        private CarIncident GetCarIncident()
        {
            return new CarIncident()
            {
                VIN = Guid.NewGuid().ToString(),
                Model = "Model" + DateTime.UtcNow.Second,
                OwnerNumber = Random.Shared.Next(100).ToString()
            };
        }

        private FlatIncident GetFlatIncident()
        {
            return new FlatIncident()
            {
                OwnerNumber = Random.Shared.Next(100).ToString()
            };
        }

        private void SendMessage(byte[] eventData, IProducer<string, byte[]> producer, string topic)
        {
            try
            {
                var deliveryResult = producer.ProduceAsync(
                    topic, 
                    new Message<string, byte[]> { Key = Guid.NewGuid().ToString(), Value = eventData })
                    .GetAwaiter().GetResult();
                
                Console.WriteLine($"Message delivered to {deliveryResult.TopicPartitionOffset}");
            }
            catch (ProduceException<string, byte[]> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        }
    }
}
