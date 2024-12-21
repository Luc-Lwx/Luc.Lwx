using Luc.Lwx.Example.Api.LwxAuthPolicies;
using Luc.Lwx.Interface;
using Luc.Lwx.LwxActivityLog;

namespace Luc.Lwx.Example.Api.LwxEndpoints.ExampleParamInPath.ParamId;


public static partial class EndpointCancel
{ 
    [LwxEndpoint(
      Path = "DELETE /apimanager-prefix/example-param-in-path/{id}/cancel",
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
      ShortDescription = "Cancel the example proccess"
    )]
    public static void Execute
    ( 
      HttpContext ctx 
    ) 
    {
      // teste    
    }
}