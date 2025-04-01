using DataReader.Processors;
using DTO.InsuranceIncidents;

namespace DataReader
{
    public class ProcessorFactory
    {
        internal static IProcessor CreateProcessor(string processorType)
        {
            return processorType switch
            {
                nameof(CarIncident) => new CarIncidentProcessor(),
                nameof(FlatIncident) => new FlatIncidentProcessor(),
                nameof(HealthIncident) => new HealthIncidentProcessor(),
                _ => throw new ArgumentException("Invalid processor type " + processorType)
            };
        }
    }
}
