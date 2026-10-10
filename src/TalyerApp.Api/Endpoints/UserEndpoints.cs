using System.Security.Claims;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Application.Dto;

namespace TalyerApp.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users/me", async (HttpContext httpContext, IUserRep userRepository) =>
        {
            var userIdValue = httpContext.User.FindFirstValue("talyer_user_id");

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Results.Unauthorized();
            }

            var userResult = await userRepository.GetByIdAsync(userId);

            return userResult.IsSuccess
                ? Results.Ok(UserDto.FromEntity(userResult.Value))
                : Results.NotFound();
        })
        .RequireAuthorization()
        .WithName("GetCurrentUser");

        return app;
    }
}
