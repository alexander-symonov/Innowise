using Data.Core.Mongo.Queries.User;
using Data.Core.Queries.User;
using MongoDB.Driver;
using ReportsAPI.Settings;
using Swashbuckle.AspNetCore.Annotations;

namespace ReportsAPI.Modules.User
{
    public static class UserModule
    {
        public static void RegisterUserModule(this IServiceCollection services, MongoDbSettings dbSettings)
        {
            if (dbSettings == null)
            {
                throw new ArgumentNullException(nameof(dbSettings));
            }

            services.AddTransient<IGetUsersListQuery>(x => new GetUsersListQuery(
                x.GetService<IMongoDatabase>(),
                dbSettings.UsersCollectionName));
        }

        public static void RegisterUserEndpoints(this IEndpointRouteBuilder routes, string apiUrl)
        {
            routes.MapGet(
                apiUrl + "/{id:guid}",
                async (Guid id) => {
                    //var result = await service.GetUserAsync(id);
                    //return result != null ? Results.Ok(result) : Results.NotFound();
                }).WithMetadata(new SwaggerOperationAttribute(summary: "Get user info"));

            routes.MapGet(
                apiUrl,
                async (IGetUsersListQuery query) => {
                    var result = await query.ExecuteAsync(CancellationToken.None);

                    return Results.Ok(result);
                }).WithMetadata(new SwaggerOperationAttribute(
                    summary: "Get all users"));
        }
    }
}
