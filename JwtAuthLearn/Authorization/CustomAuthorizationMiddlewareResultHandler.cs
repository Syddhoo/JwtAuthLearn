using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace JwtAuthLearn.Authorization;

public class CustomAuthorizationMiddlewareResultHandler
    : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler
        _defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Forbidden)
        {
            var endpoint = context.GetEndpoint();

            var authorizeData =
                endpoint?.Metadata.GetOrderedMetadata<IAuthorizeData>();

            var policyName = authorizeData?
                .Select(data => data.Policy)
                .FirstOrDefault(name =>
                    !string.IsNullOrWhiteSpace(name));

            var message = policyName switch
            {
                "AdminOnly" =>
                    "You're not an Admin.",

                "ManagerOrHigher" =>
                    "You're not a Manager or higher.",

                "TeamLeadDeveloperOrHigher" =>
                    "You're not a TeamLead/Developer or higher.",

                _ =>
                    "You don't have permission to access this resource."
            };

            context.Response.StatusCode =
                StatusCodes.Status403Forbidden;

            await context.Response.WriteAsJsonAsync(new
            {
                message
            });

            return;
        }

        await _defaultHandler.HandleAsync(
            next,
            context,
            policy,
            authorizeResult);
    }
}