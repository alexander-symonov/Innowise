using DTO.InsuranceIncidents;

namespace DataReader.Processors
{
    internal class FlatIncidentProcessor : IProcessor
    {
        public Task ProcessAsync(byte[] data, CancellationToken cancellationToken)
        {
            var message = FlatIncident.Parser.ParseFrom(data);

            Console.WriteLine($"Processing FlatIncident: {message}");
            // TODO: Implement processing logic here

            return Task.CompletedTask;
        }
    }
}