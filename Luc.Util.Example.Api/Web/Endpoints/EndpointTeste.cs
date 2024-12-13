using Luc.Util.Example.Api.Web.AuthPolicies;
using Luc.Util.Web;

namespace Luc.Util.Example.Api.Web.Endpoints;

[LucEndpoint(
  Path = "GET /prefixo-no-api-manager/teste",
  AuthPolicy = typeof(AuthPolicyExample001),
  SwaggerFuncName = "nome da api de testes",
  SwaggerFuncSummary = "sumário da api de testes",
  SwaggerFuncDescription = "descrição da api de testes",
  SwaggerGroupTitle = "grupo da api de testes"
)]
public static class EndpointTeste 
{ 
  public static void Execute() 
  {
    // teste    
  }
}

 