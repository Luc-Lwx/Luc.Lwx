using System.Text;
using Luc.Web;
using Luc.Web.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Luc.Web.Example.Api.Web.AuthSchemes;


public static partial class AuthSchemeExample001
{ 
  [LucAuthScheme]
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
