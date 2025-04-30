using Google.Protobuf;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Data.Core.Mongo.Commands
{
    public abstract class StoreCommandBase
    {
        protected readonly IMongoCollection<BsonDocument> _collection;

        public StoreCommandBase(
            IMongoClient mongoClient,
            string databaseName,
            string collectionName
            )
        {
            _collection = mongoClient.GetDatabase(databaseName)
                .GetCollection<BsonDocument>(collectionName);
        }

        public async Task ExecuteAsync(IMessage healthIncident, CancellationToken cancellationToken)
        {
            var json = Google.Protobuf.JsonFormatter.Default.Format(healthIncident);
            var bson = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(json);

            await _collection.InsertOneAsync(bson, null, cancellationToken);
        }
    }
}
