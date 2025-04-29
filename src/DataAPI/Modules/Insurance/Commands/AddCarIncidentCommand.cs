using Confluent.Kafka;
using DataAPI.Modules.Insurance.Models;
using Microsoft.Extensions.Options;

namespace DataAPI.Modules.Insurance.Commands
{
    public class AddCarIncidentCommand : KafkaCommandBase, IAddCarIncidentCommand
    {
        string _topicName;

        public AddCarIncidentCommand(
            IProducer<string, byte[]> producer,
            IOptions<Configuration> options)
            :base(producer)
        {
            var topicNamePrefix = options.Value.KafkaTopicNamePrefix;
            if (string.IsNullOrEmpty(topicNamePrefix)) {
                throw new ArgumentNullException(nameof(options.Value.KafkaTopicNamePrefix));
            }

            _topicName = topicNamePrefix + nameof(CarIncident);
        }
        public Task<bool> ExecuteAsync(CarIncident context)
        {
            var message = new DTO.InsuranceIncidents.CarIncident() { 
                Model = context.Model,
                OwnerNumber = context.OwnerNumber,
                VIN = context.VIN,
                Tags = { context.Tags }
            };

            return SendMessage(Serialize(message), _producer, _topicName);
        }
    }
}
