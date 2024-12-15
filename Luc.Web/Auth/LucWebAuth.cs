using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using Luc.Web.SetupState;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Luc.Web.Auth;

internal static class LucWebAuth
{
    internal static void LucDefaultAccessPolicyIsDenied(this WebApplicationBuilder builder)
    {
      var setupState = builder.GetLucSetupState();
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
        }).AddScheme<AuthenticationSchemeOptions, DefaultAuthHandler>("DefaultScheme", null);

        setupState.IsAuthSchemeInitialized = true;
      }
    }   
}

public class DefaultAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    [SuppressMessage("","IDE0290")]
    public DefaultAuthHandler
    (
      IOptionsMonitor<AuthenticationSchemeOptions> options, 
      ILoggerFactory logger,
      UrlEncoder encoder
    ) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return Task.FromResult(AuthenticateResult.Fail("Access Denied"));
    }
}


