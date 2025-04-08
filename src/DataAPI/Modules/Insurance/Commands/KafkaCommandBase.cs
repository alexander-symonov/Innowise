using Confluent.Kafka;
using Google.Protobuf;

namespace DataAPI.Modules.Insurance.Commands
{
    public class KafkaCommandBase
    {
        protected IProducer<string, byte[]> _producer;

        public KafkaCommandBase(IProducer<string, byte[]> producer)
        {
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
        }

        protected virtual byte[] Serialize(IMessage record)
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

        protected virtual async Task<bool> SendMessage(byte[] eventData, IProducer<string, byte[]> producer, string topic)
        {
            try
            {
                var deliveryResult = await producer.ProduceAsync(
                    topic,
                    new Message<string, byte[]> { Key = Guid.NewGuid().ToString(), Value = eventData });

                //Console.WriteLine($"Message delivered to {deliveryResult.TopicPartitionOffset}");
            }
            catch (ProduceException<string, byte[]> e)
            {
                //Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                return false;
            }

            return true;
        }
    }
}
