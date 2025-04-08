using Confluent.Kafka;
using DataAPI.Modules.Insurance.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;

namespace DataAPI.Modules.Insurance.Commands
{
    public class AddHealthIncidentCommand : KafkaCommandBase, IAddHealthIncidentCommand
    {
        string _topicName;

        public AddHealthIncidentCommand(
            IProducer<string, byte[]> producer,
            IOptions<Configuration> options)
            :base(producer)
        {
            var topicNamePrefix = options.Value.KafkaTopicNamePrefix;
            if (string.IsNullOrEmpty(topicNamePrefix)) {
                throw new ArgumentNullException(nameof(options.Value.KafkaTopicNamePrefix));
            }

            _topicName = topicNamePrefix + nameof(HealthIncident);
        }
        public Task<bool> ExecuteAsync(HealthIncident context)
        {
            var message = new DTO.InsuranceIncidents.HealthIncident()
            {
                FirstName = context.FirstName,
                LastName = context.LastName,
                Patronymic = context.Patronymic,
                BirthDate = context.BirthDate.ToTimestamp(),
                Address = new DTO.InsuranceIncidents.HealthIncident.Types.HumanAddress()
                {
                    PostalCode = context.Address.PostalCode,
                    Country = context.Address.Country,
                    City = context.Address.City,
                    Street = context.Address.Street,
                    Building = context.Address.Building
                }
            };

            return SendMessage(Serialize(message), _producer, _topicName);
        }
    }
}
