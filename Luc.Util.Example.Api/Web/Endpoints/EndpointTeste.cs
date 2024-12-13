using Luc.Util.Web;

namespace Luc.Util.Example.Api.Web.Endpoints;

[LucEndpoint(
  Path = "GET /prefixo-no-api-manager/teste",
  GeneratedMethodName = "MapExampleEndpoints",
  AuthPolicy = "ABC",
  SwaggerFuncName = "nome da api de testes",
  SwaggerFuncSummary = "sumário da api de testes",
  SwaggerFuncDescription = "descrição da api de testes",
  SwaggerGroupTitle = "grupo da api de testes",
  LowMaintanability_ParameterInPath_Justification = "Não é possível evitar o parâmetro na URL"    
)]
public class EndpointTeste 
{ 
  public static void Execute() 
  {
    
  }
}

 