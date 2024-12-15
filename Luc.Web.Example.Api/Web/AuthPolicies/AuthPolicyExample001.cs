using Luc.Web.Interface;
using Microsoft.AspNetCore.Authorization;

namespace Luc.Web.Example.Api.Web.AuthPolicies;

public static partial class AuthPolicyExample001
{
  [LucAuthPolicy]
  public static void Configure( AuthorizationPolicyBuilder policy ) 
  { 
    policy.RequireAuthenticatedUser();
  }
}
