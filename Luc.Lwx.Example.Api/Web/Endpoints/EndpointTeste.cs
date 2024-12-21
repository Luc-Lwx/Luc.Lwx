using Luc.Lwx.Example.Api.Web.AuthPolicies;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;

namespace Luc.Lwx.Example.Api.Web.Endpoints;


public static partial class EndpointTeste 
{ 
    [LwxEndpoint(
      Path = "GET /prefixo-no-api-manager/teste",
      AuthPolicy = typeof(AuthPolicyExample001),
      SwaggerFuncName = "nome da api de testes",
      SwaggerFuncSummary = "sumário da api de testes",
      SwaggerFuncDescription = "descrição da api de testes",
      SwaggerGroupTitle = "grupo da api de testes"
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish
    )]
    public static void Execute
    ( 
      HttpContext ctx 
    ) 
    {
      // teste    
    }
}