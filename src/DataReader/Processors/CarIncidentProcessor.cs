using DTO.InsuranceIncidents;

namespace DataReader.Processors
{
    internal class CarIncidentProcessor : IProcessor
    {
        public Task ProcessAsync(byte[] data, CancellationToken cancellationToken)
        {
            var message = CarIncident.Parser.ParseFrom(data);

            Console.WriteLine($"Processing CarIncident: {message}");
            // TODO: Implement processing logic here

            return Task.CompletedTask;
        }
    }
}
