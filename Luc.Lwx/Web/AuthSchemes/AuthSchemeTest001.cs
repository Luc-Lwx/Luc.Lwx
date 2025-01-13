using Luc.Lwx.Interface;
using Luc.Lwx.LwxAuth;
using Luc.Lwx.Util;
using Microsoft.AspNetCore.Authentication;

namespace Luc.Lwx.Web.AuthSchemes;

/// <summary>
/// This class is used to configure an authentication scheme for testing purposes
/// 
/// You can use it, by including the following code in your Program.cs file:
/// <code>
/// builder.MapAuthSchemes_LwxTestIssuer();
/// </code>
/// 
/// In your tests you can generate a token using the following code:
/// <code>
/// var exampleToken = AuthSchemeTest001.TestIssuer.CreateToken(
///     issuer: "https://example.com", 
///     audience: "myapp", 
///     new Claim("preferred_username", "lucas@example.com"),
///     new Claim("abc", "cde"),
///     new Claim("cde", "efg")
/// );
/// </code>
/// </summary>
public static partial class AuthSchemeTest001 
{
    public static LwxAuthIssuerForTesting TestIssuer { get; set; } = new LwxAuthIssuerForTesting( (new Random()).LwxRandomHexString(32) );

    [LwxAuthScheme(GeneratedMethodName ="MapAuthSchemes_LwxTestIssuer")]
    public static void Configure( AuthenticationBuilder authBuilder ) 
    {        
        authBuilder.AddJwtBearer("LwxTestIssuer", options =>
        {            
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = TestIssuer.JwtSecurityKey,
                ValidAlgorithms = [ TestIssuer.JwtSecurityAlgorithm ],
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.FromSeconds(30)
            };
        });
    }
}