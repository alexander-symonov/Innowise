using Data.Core.Commands;
using DTO.InsuranceIncidents;
using System.Text.Json;

namespace DataReader.Processors
{
    internal class CarIncidentProcessor : IProcessor
    {
        private IStoreCarIncidentCommand _storeCarIncidentCommand;

        public CarIncidentProcessor(IStoreCarIncidentCommand storeCarIncidentCommand)
        {
            _storeCarIncidentCommand = storeCarIncidentCommand;
        }

        public async Task ProcessAsync(byte[] data, CancellationToken cancellationToken)
        {
            var message = CarIncident.Parser.ParseFrom(data);

            await _storeCarIncidentCommand.ExecuteAsync(message, cancellationToken);

            Console.WriteLine($"Processing CarIncident as JSON: {JsonSerializer.Serialize(message)}");
            
            // TODO: Implement further processing logic here
        }
    }
}
