using Luc.Lwx.Example.Api.LwxAuthPolicies;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;

namespace Luc.Lwx.Example.Api.LwxEndpoints.ExampleParamInPath.ParamId;


public static partial class EndpointStep1
{ 
    [LwxEndpoint(
      Path = "PUT /apimanager-prefix/example-param-in-path/{id}/step1",
      AuthPolicy = typeof(AuthPolicyPublic),
      SwaggerFuncName = "nome da api de testes",
      SwaggerFuncSummary = "sumário da api de testes",
      SwaggerFuncDescription = "descrição da api de testes",
      SwaggerGroupTitle = "grupo da api de testes",
      LowMaintanability_ParameterInPath_Justification="This is an example. Although not recomended, someone may need this to implement old style endpoints."
    )]
    [LwxActivityLog(
      Imporance = LwxActivityImportance.High,
      Step = LwxActionStep.Finish,
      ShortDescription = "Step 1 of the example proccess"
    )]
    public static void Execute
    ( 
      HttpContext ctx 
    ) 
    {
      // teste    
    }
}