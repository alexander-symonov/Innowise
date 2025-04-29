using Confluent.Kafka;
using DataAPI.Modules.Insurance.Models;
using Microsoft.Extensions.Options;

namespace DataAPI.Modules.Insurance.Commands
{
    public class AddFlatIncidentCommand : KafkaCommandBase, IAddFlatIncidentCommand
    {        
        string _topicName;

        public AddFlatIncidentCommand(
            IProducer<string, byte[]> producer,
            IOptions<Configuration> options)
            :base(producer)
        {
            var topicNamePrefix = options.Value.KafkaTopicNamePrefix;
            if (string.IsNullOrEmpty(topicNamePrefix)) {
                throw new ArgumentNullException(nameof(options.Value.KafkaTopicNamePrefix));
            }

            _topicName = topicNamePrefix + nameof(FlatIncident);
        }
        public Task<bool> ExecuteAsync(FlatIncident context)
        {
            var message = new DTO.InsuranceIncidents.FlatIncident
            {
                Address = new DTO.InsuranceIncidents.FlatIncident.Types.FlatAddress
                {
                    PostalCode = context.Address.PostalCode,
                    Country = context.Address.Country,
                    City = context.Address.City,
                    Street = context.Address.Street,
                    Building = context.Address.Building
                },
                OwnerNumber = context.OwnerNumber,
                Tags = { context.Tags }
            };

            return SendMessage(Serialize(message), _producer, _topicName);
        }
    }
}
