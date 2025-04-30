using Data.Core.Commands;
using DTO.InsuranceIncidents;
using MongoDB.Driver;

namespace Data.Core.Mongo.Commands
{
    public class StoreCarIncidentCommand : StoreCommandBase, IStoreCarIncidentCommand
    {
        public StoreCarIncidentCommand(
            IMongoClient mongoClient,
            string databaseName,
            string collectionName
            ):base(mongoClient, databaseName, collectionName)
        {}

        public async Task ExecuteAsync(CarIncident carIncident, CancellationToken cancellationToken)
        {
            await base.ExecuteAsync(carIncident, cancellationToken);
        }
    }
}
