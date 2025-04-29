using Data.Core.Commands;
using DTO.InsuranceIncidents;
using System.Text.Json;

namespace DataReader.Processors
{
    internal class CarIncidentProcessor : IProcessor
    {
        //private ISaveInsuranceDataService _saveInsuranceDataService;
        private IStoreCarIncidentCommand _storeCarIncidentCommand;

        public CarIncidentProcessor(
            //ISaveInsuranceDataService saveInsuranceDataService,
            IStoreCarIncidentCommand storeCarIncidentCommand
            )
        {
            //this._saveInsuranceDataService = saveInsuranceDataService;
            this._storeCarIncidentCommand = storeCarIncidentCommand;
        }

        public async Task ProcessAsync(byte[] data, CancellationToken cancellationToken)
        {
            var message = CarIncident.Parser.ParseFrom(data);
            var json = JsonSerializer.Serialize(message);

            await _storeCarIncidentCommand.ExecuteAsync(message, cancellationToken);
            //_saveInsuranceDataService.SaveCarIncident(message);
            Console.WriteLine($"Processing CarIncident as JSON: {json}");
            
            // TODO: Implement further processing logic here
        }
    }
}
