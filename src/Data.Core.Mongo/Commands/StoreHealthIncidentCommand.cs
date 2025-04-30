using Data.Core.Commands;
using DTO.InsuranceIncidents;
using MongoDB.Driver;

namespace Data.Core.Mongo.Commands
{
    public class StoreHealthIncidentCommand : StoreCommandBase, IStoreHealthIncidentCommand
    {
        public StoreHealthIncidentCommand(
            IMongoClient mongoClient,
            string databaseName,
            string collectionName
            ): base(mongoClient, databaseName, collectionName)
        { }

        public async Task ExecuteAsync(HealthIncident healthIncident, CancellationToken cancellationToken)
        {
            await base.ExecuteAsync(healthIncident, cancellationToken);
        }
    }
}
