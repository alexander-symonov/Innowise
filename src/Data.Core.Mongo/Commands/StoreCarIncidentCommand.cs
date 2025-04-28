using Data.Core.Commands;
using DTO.InsuranceIncidents;
using MongoDB.Driver;

namespace Data.Core.Mongo.Commands
{
    public class StoreCarIncidentCommand : IStoreCarIncidentCommand
    {
        IMongoCollection<CarIncident> _carIncidentsCollection;

        public StoreCarIncidentCommand(IMongoClient mongoClient)
        {
            _carIncidentsCollection = mongoClient.GetDatabase("InsuranceIncidents")
                .GetCollection<CarIncident>("CarIncidents");
        }

        public async Task ExecuteAsync(CarIncident carIncident, CancellationToken cancellationToken)
        {
            await _carIncidentsCollection.InsertOneAsync(carIncident, null, cancellationToken);
        }
    }
}
