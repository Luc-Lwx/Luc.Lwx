using System.Text;
using Luc.Util.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Luc.Util.Example.Api.Web.AuthSchemes;

[LucAuthScheme]
public static partial class AuthSchemeExample001
{ 
  public static void Configure( AuthenticationBuilder authBuilder ) 
  {
    authBuilder.AddJwtBearer
    (
      options => 
      {
        options.Authority = "https://your-issuer";
        options.Audience = "https://your-audience";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true            
        };
      }
    );
  }
}
