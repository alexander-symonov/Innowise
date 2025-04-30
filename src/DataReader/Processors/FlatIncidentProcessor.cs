using Data.Core.Commands;
using DTO.InsuranceIncidents;
using System.Text.Json;

namespace DataReader.Processors
{
    internal class FlatIncidentProcessor : IProcessor
    {
        private readonly IStoreFlatIncidentCommand _storeFlatIncidentCommand;
        public FlatIncidentProcessor(IStoreFlatIncidentCommand storeFlatIncidentCommand)
        {
            _storeFlatIncidentCommand = storeFlatIncidentCommand;
        }

        public async Task ProcessAsync(byte[] data, CancellationToken cancellationToken)
        {
            var message = FlatIncident.Parser.ParseFrom(data);

            await _storeFlatIncidentCommand.ExecuteAsync(message, cancellationToken);

            Console.WriteLine($"Processing FlatIncident: {JsonSerializer.Serialize(message)}");
            // TODO: Implement processing logic here
        }
    }
}