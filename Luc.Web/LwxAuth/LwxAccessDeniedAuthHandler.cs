using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Luc.Web.LwxAuth;

public class LwxAccessDeniedAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    [SuppressMessage("","IDE0290")]
    public LwxAccessDeniedAuthHandler
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


