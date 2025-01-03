using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Luc.Lwx.LwxAuth;

/// <summary>
/// This class is used to create fake JWT tokens for testing purposes.
/// </summary>
public class LwxAuthIssuerForTesting
{
    private readonly SigningCredentials _jwtSigningCredentials;
    private readonly string _jwtSecurityAlgorithm;
    private SymmetricSecurityKey _jwtSecurityKey;

    /// <summary>
    /// Constructor for the LwxAuthIssuerForTesting class.
    /// </summary>
    public LwxAuthIssuerForTesting
    (
        string jwtSecret        
    )
    {
        if( jwtSecret.Length != 32) 
        {
            throw new ArgumentException("Security key must be 32 characters long (I suggest a random string).");
        }
        _jwtSecurityAlgorithm = SecurityAlgorithms.HmacSha256;
        _jwtSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        _jwtSigningCredentials = new SigningCredentials(_jwtSecurityKey, _jwtSecurityAlgorithm);        
    }
    
    public string JwtSecurityAlgorithm => _jwtSecurityAlgorithm;    
    public SymmetricSecurityKey JwtSecurityKey => _jwtSecurityKey;
    public SigningCredentials JwtSigningCredentials => _jwtSigningCredentials;


    public string CreateToken
    (
        string issuer,
        string audience,        
        params Claim[] claims
    )
    {
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: _jwtSigningCredentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
