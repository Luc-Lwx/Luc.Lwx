using Luc.Util.Example.Api.Web.AuthPolicies;
using Luc.Util.Web;

namespace Luc.Util.Example.Api.Web.Endpoints;

[LucEndpoint(
  Path = "GET /prefixo-no-api-manager/teste",
  AuthPolicy = AuthPolicyExample001.Id,
  SwaggerFuncName = "nome da api de testes",
  SwaggerFuncSummary = "sumário da api de testes",
  SwaggerFuncDescription = "descrição da api de testes",
  SwaggerGroupTitle = "grupo da api de testes",
  LowMaintanability_ParameterInPath_Justification = "Não é possível evitar o parâmetro na URL"    
)]
public static class EndpointTeste 
{ 
  public static void Execute() 
  {
    // teste    
  }
}

 