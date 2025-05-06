using Data.Core.Queries.User;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Data.Core.Mongo.Queries.User
{
    public class GetUsersListQuery : IGetUsersListQuery
    {
        //private readonly IMongoCollection<DTO.QueryModels.User.User> _usersCollection;
        private readonly IMongoCollection<BsonDocument> _bsonCollection;
        public GetUsersListQuery(IMongoDatabase database, string userCollectionName)
        {
            //_usersCollection = database.GetCollection<DTO.QueryModels.User.User>(userCollectionName);
            _bsonCollection = database.GetCollection<BsonDocument>(userCollectionName);
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DTO.QueryModels.User.User>> ExecuteAsync(CancellationToken cancellationToken)
        {
            var bsonDocuments = await _bsonCollection.Find(_ => true).ToListAsync(cancellationToken);
            var users = bsonDocuments.Select(doc => new DTO.QueryModels.User.User
            {
                Id = doc.GetValue("_id").AsInt32,
                Name = doc.GetValue("name").AsString,
                BirthDate = doc.GetValue("birthdate").ToUniversalTime()
            }).ToList();

            return users;

            //var users = await _usersCollection.Find(_ => true).ToListAsync(cancellationToken);
            //return users.Select(user => new DTO.QueryModels.User.User
            //{
            //    Id = user.Id,
            //    Name = user.Name,
            //    BirthDate = user.BirthDate
            //});
        }
    }
}
