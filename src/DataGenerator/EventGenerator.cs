using Confluent.Kafka;
using DTO.InsuranceIncidents;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Text;

namespace DataGenerator
{
    public class EventGenerator
    {
        public async Task GenerateForKafka(string brokerList, string topicNamePrefix, string type, int count, int delayMs, StateMediator state)
        {
            var producerConfig = new ProducerConfig { BootstrapServers = brokerList };
            using var producer = new ProducerBuilder<string, byte[]>(producerConfig).Build();
            var topicName = topicNamePrefix + type;

            for (int i = 0; i < count; i++)
            {
                if (state.Token.IsCancellationRequested)
                {
                    Console.WriteLine("Cancellation requested. Stopping message generation.");
                    break;
                }

                var message = GenerateEvent(type, i);
                
                await Task.WhenAll(
                    SendMessageToKafka(Serialize(message), producer, topicName),
                    Task.Delay(delayMs));

                state.SucsessCounter++;
            }
        }

        private async Task SendMessageToKafka(byte[] eventData, IProducer<string, byte[]> producer, string topic)
        {
            try
            {
                var deliveryResult = await producer.ProduceAsync(
                    topic,
                    new Message<string, byte[]> { Key = Guid.NewGuid().ToString(), Value = eventData });

                Console.WriteLine($"Message delivered to {deliveryResult.TopicPartitionOffset}");
            }
            catch (ProduceException<string, byte[]> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        }

        public async Task SendMessageToApi(string apiUrl, string type, int count, int delayMs, StateMediator state)
        {
            var api = apiUrl + "/" + type.ToLower().Replace("incident", "");
            using (var client = new HttpClient())
            {
                var formatter = new Google.Protobuf.JsonFormatter(new JsonFormatter.Settings(true));

                Console.WriteLine($"Sending {count} messages to API: {api}");

                for (int i = 0; i < count; i++)
                {
                    if(state.Token.IsCancellationRequested)
                    {
                        Console.WriteLine("Cancellation requested. Stopping message generation.");
                        break;
                    }
                    var message = GenerateEvent(type, i);
                    var jsonMessage = formatter.Format(message);
                    var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
                    
                    var requestTask = client.PostAsync(api, content);
                    await Task.WhenAll(requestTask, Task.Delay(delayMs));

                    if (requestTask.Result.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Message {i} sent to API successfully.");
                        state.SucsessCounter++;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send message {i} to API: {requestTask.Result.StatusCode}");
                        state.FailedCounter++;
                    }
                }
            }
        }

        private IMessage GenerateEvent(string type, int number)
        {
            return true switch
            {
                true when typeof(CarIncident).Name == type => GetCarIncident(),
                true when typeof(FlatIncident).Name == type => GetFlatIncident(),
                true when typeof(HealthIncident).Name == type => GetHealthIncident(number),
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

        private HealthIncident GetHealthIncident(int number)
        {
            return new HealthIncident()
            {
                FirstName = "FirstName" + number,
                LastName = "LastName" + number,
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
            var now = DateTime.UtcNow;
            return new FlatIncident()
            {
                OwnerNumber = Random.Shared.Next(100).ToString(),
                Address = new FlatIncident.Types.FlatAddress()
                {
                    PostalCode = now.Millisecond.ToString(),
                    Country = "Country" + now.Second,
                    City = "City" + now.Second,
                    Street = "Street" + now.Second,
                    Building = "Building" + now.Second
                }
            };
        }
    }
}
