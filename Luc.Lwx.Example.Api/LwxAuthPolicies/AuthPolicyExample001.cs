using Luc.Lwx.Interface;
using Microsoft.AspNetCore.Authorization;

namespace Luc.Lwx.Example.Api.LwxAuthPolicies;

public static partial class AuthPolicyExample001
{
  [LwxAuthPolicy]
  public static void Configure( AuthorizationPolicyBuilder policy ) 
  { 
    policy.RequireAuthenticatedUser();
  }
}
