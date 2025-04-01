using DTO.InsuranceIncidents;

namespace DataReader.Processors
{
    internal class HealthIncidentProcessor : IProcessor
    {
        public Task ProcessAsync(byte[] data, CancellationToken cancellationToken)
        {
            var message = HealthIncident.Parser.ParseFrom(data);

            Console.WriteLine($"Processing HelthIncident: {message}");
            // TODO: Implement processing logic here

            return Task.CompletedTask;
        }
    }
}