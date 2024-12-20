using Luc.Web.LwxSetupState;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Luc.Web.LwxAuth;

internal static class LwxAuthExtensions
{
    internal static void LwxDefaultAccessPolicyIsDenied(this WebApplicationBuilder builder)
    {
      var setupState = builder.LwxGetSetupState();
      if (!setupState.IsAuthSchemeInitialized)
      {
        builder.Services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        }).AddAuthorizationBuilder();

        builder.Services.Configure<AuthorizationOptions>(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = "DefaultScheme";
        }).AddScheme<AuthenticationSchemeOptions, LwxAccessDeniedAuthHandler>("DefaultScheme", null);

        setupState.IsAuthSchemeInitialized = true;
      }
    }   
}


