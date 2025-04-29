using Data.Core.Commands;
using DTO.InsuranceIncidents;
using MongoDB.Driver;

namespace Data.Core.Mongo.Commands
{
    public class StoreCarIncidentCommand : IStoreCarIncidentCommand
    {
        IMongoCollection<CarIncident> _carIncidentsCollection;

        public StoreCarIncidentCommand(
            IMongoClient mongoClient,
            string databaseName = "insurance",
            string collectionName = "car_incidents"
            )
        {
            _carIncidentsCollection = mongoClient.GetDatabase(databaseName)
                .GetCollection<CarIncident>(collectionName);
        }

        public async Task ExecuteAsync(CarIncident carIncident, CancellationToken cancellationToken)
        {
            await _carIncidentsCollection.InsertOneAsync(carIncident, null, cancellationToken);
        }
    }
}
