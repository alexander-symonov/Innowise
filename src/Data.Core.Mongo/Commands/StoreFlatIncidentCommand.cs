using Data.Core.Commands;
using DTO.InsuranceIncidents;
using MongoDB.Driver;

namespace Data.Core.Mongo.Commands
{
    public class StoreFlatIncidentCommand : StoreCommandBase, IStoreFlatIncidentCommand
    {
        public StoreFlatIncidentCommand(
            IMongoClient mongoClient,
            string databaseName,
            string collectionName
            ): base(mongoClient, databaseName, collectionName)
        { }

        public async Task ExecuteAsync(FlatIncident flatIncident, CancellationToken cancellationToken)
        {
            await base.ExecuteAsync(flatIncident, cancellationToken);
        }
    }
}
