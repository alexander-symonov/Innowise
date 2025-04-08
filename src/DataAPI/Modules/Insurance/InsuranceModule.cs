using DataAPI.Modules.Insurance.Commands;
using DataAPI.Modules.Insurance.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace DataAPI.Modules.Insurance
{
    public static class InsuranceModule
    {
        public static IServiceCollection RegisterInsuranceModule(this IServiceCollection services)
        {            
            services.AddScoped<IAddCarIncidentCommand, AddCarIncidentCommand>();
            services.AddScoped<IAddFlatIncidentCommand, AddFlatIncidentCommand>();
            services.AddScoped<IAddHealthIncidentCommand, AddHealthIncidentCommand>();

            return services;
        }

        public static void RegisterInsuranceEndpoints(this IEndpointRouteBuilder routes, string apiUrl)
        {
            routes.MapPost(
                apiUrl + "/car",
                async (CarIncident model, IAddCarIncidentCommand command) =>
                {
                    var result = await command.ExecuteAsync(model);
                    return result ? Results.Ok() : Results.BadRequest();    
                })
                .WithMetadata(new SwaggerOperationAttribute(summary: "Add car insurance record"))
                .Produces(200)
                .Produces(400);

            routes.MapPost(
                apiUrl + "/flat",
                async (FlatIncident model, IAddFlatIncidentCommand command) =>
                {
                    var result = await command.ExecuteAsync(model);
                    return result ? Results.Ok() : Results.BadRequest();
                })
                .WithMetadata(new SwaggerOperationAttribute(summary: "Add flat insurance record"))
                .Produces(200)
                .Produces(400);

            routes.MapPost(
                apiUrl + "/health",
                async (HealthIncident model, IAddHealthIncidentCommand command) =>
                {
                    var result = await command.ExecuteAsync(model);
                    return result ? Results.Ok() : Results.BadRequest();
                })
                .WithMetadata(new SwaggerOperationAttribute(summary: "Add health insurance record"))
                .Produces(200)
                .Produces(400);
        }
    }
}
