using Data.Core.Commands;
using DataReader.Processors;
using DTO.InsuranceIncidents;
using Microsoft.Extensions.DependencyInjection;

namespace DataReader
{
    public class ProcessorFactory: IProcessorFactory
    {
        private ServiceProvider _serviceProvider;
        public ProcessorFactory(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IProcessor CreateProcessor(string processorType)
        {
            return processorType switch
            {
                nameof(CarIncident) => new CarIncidentProcessor(_serviceProvider.GetService<IStoreCarIncidentCommand>()),
                nameof(FlatIncident) => new FlatIncidentProcessor(),
                nameof(HealthIncident) => new HealthIncidentProcessor(),
                _ => throw new ArgumentException("Invalid processor type " + processorType)
            };
        }
    }
}
