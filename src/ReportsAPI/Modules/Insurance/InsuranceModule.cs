using Swashbuckle.AspNetCore.Annotations;

namespace ReportsAPI.Modules.Insurance
{
    public static class InsuranceModule
    {
        public static void RegisterInsuranceModule(this IServiceCollection services)
        {

        }

        public static void RegisterInsuranceEndpoints(this IEndpointRouteBuilder routes, string apiUrl)
        {
            routes.MapGet(
                apiUrl + "/{id:guid}",
                async (Guid id) => {
                    //var result = await service.GetPatientAsync(id);
                    //return result != null ? Results.Ok(result) : Results.NotFound();
                }).WithMetadata(new SwaggerOperationAttribute(summary: "Find patient by GUID"));

            routes.MapGet(
                apiUrl + "/search",
                async () => {
                    //var result = await service.GetPatientsByBirthDateAsync(dateString.Split(','));
                    //return result.Any() ? Results.Ok(result) : Results.NotFound();
                }).WithMetadata(new SwaggerOperationAttribute(
                    summary: "Find patient by date"));
        }
    }
}
