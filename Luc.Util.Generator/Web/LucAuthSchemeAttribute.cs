namespace Luc.Util.Web;

/// <summary>
/// This attribute is used to define a class that will be used to configure an authentication scheme.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class LucAuthSchemeAttribute : Attribute
{  
  /// <summary>
  /// The name of the authentication scheme.
  /// </summary>
  public required string Name { get; set; }

  /// <summary>
  /// This controls the generated method name. 
  /// </summary>
  public string? GeneratedMethodName { get; set; } = null;
}
