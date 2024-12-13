using Luc.Web;
using Microsoft.AspNetCore.Authorization;

namespace Luc.Web.Example.Api.Web.AuthPolicies;

[LucAuthPolicy]
public static partial class AuthPolicyExample001
{
  public static void Configure( AuthorizationPolicyBuilder policy ) 
  { 
    policy.RequireAuthenticatedUser();
  }
}
