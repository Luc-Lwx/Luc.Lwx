using System.Text;
using Luc.Lwx;
using Luc.Lwx.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Luc.Lwx.Example.Api.LwxAuthSchemes;


public static partial class AuthSchemeExample001
{ 
  [LwxAuthScheme()]
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
