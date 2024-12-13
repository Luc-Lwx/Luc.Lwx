
using System;

namespace Luc.Util.Web;






[AttributeUsage(AttributeTargets.Class)]
public class LucEndpointAttribute : Attribute
{ 
  /// <summary>
  /// The path in which you want to publish your endpoint (ex /example/abc)
  /// </summary>
  public required string Path { get; set; }

  /// <summary>
  /// This is useful to generate different helper classes
  /// </summary>
  public string GeneratedMethodName { get; set; } = "MapEndpoints";

  /// <summary>
  /// AuthPolicy
  /// </summary>
  public required string AuthPolicy { get; set; }

  /// <summary>
  /// When this is set, the functions with same title will be grouped on the doc
  /// </summary>
  public string? SwaggerGroupTitle { get; set; } = null;

  /// <summary>
  /// The summary is a short description that will show in the endpoint list
  /// </summary>
  public string? SwaggerFuncName { get; set; } = null;
  
  /// <summary>
  /// The summary is a short description that will show in the endpoint list
  /// </summary>
  public required string SwaggerFuncSummary { get; set; }

  /// <summary>
  /// The description is a deitaled description that will show in the detail view of the endpoint. We recomend to include some samples and other useful information to make easier for other developers to test this.
  /// </summary>
  public required string SwaggerFuncDescription { get; set; }

  /// <summary>
  /// Luc.Util enforce the naming of the class to match the endpoint path. When you need to disable this rule for your endpoint, explain to reviewers why. 
  /// </summary>
  public string? LowMaintanability_Naming_Justification { get; set; } = null;

  /// <summary>
  /// Luc.Util enforce the naming of the class to match the endpoint path. When you need to disable this rule for your endpoint, explain to reviewers why. 
  /// </summary>
  public string? LowMaintanability_ParameterInPath_Justification { get; set; } = null;

  /// <summary>
  /// Luc.Util enforce all paths to begin with the apimagner path. When you need to disable this rule for your endpoint, explain to reviewers why.
  /// </summary>
  public string? LowMaintanability_NotInApiManagerPath_Justification { get; set; } = null;

}