namespace TalyerApp.Api.Endpoints;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapUserEndpoints();
        app.MapMembershipRequestEndpoints();
        return app;
    }
}
