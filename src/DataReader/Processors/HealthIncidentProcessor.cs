using Data.Core.Commands;
using DTO.InsuranceIncidents;
using System.Text.Json;

namespace DataReader.Processors
{
    internal class HealthIncidentProcessor : IProcessor
    {
        private IStoreHealthIncidentCommand _storeHealthIncidentCommand;
        public HealthIncidentProcessor(IStoreHealthIncidentCommand storeHealthIncidentCommand)
        {
            _storeHealthIncidentCommand = storeHealthIncidentCommand;
        }

        public async Task ProcessAsync(byte[] data, CancellationToken cancellationToken)
        {
            var message = HealthIncident.Parser.ParseFrom(data);

            await _storeHealthIncidentCommand.ExecuteAsync(message, cancellationToken);

            Console.WriteLine($"Processing HelthIncident: {JsonSerializer.Serialize(message)}");
            // TODO: Implement processing logic here
        }
    }
}