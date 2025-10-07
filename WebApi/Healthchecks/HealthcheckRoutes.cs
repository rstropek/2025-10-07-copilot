namespace WebApi.Healthchecks;

public static class HealthcheckRoutes
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder api)
    {
        var healthApi = api.MapGroup("/health");

        healthApi.MapGet("/", () => Results.Ok("Healthy"));

        return api;
    }
}