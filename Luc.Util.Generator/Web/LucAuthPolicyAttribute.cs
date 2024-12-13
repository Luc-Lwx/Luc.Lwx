namespace Luc.Util.Web;

[AttributeUsage(AttributeTargets.Class)]
public class LucAuthPolicyAttribute : Attribute
{  
  public required string Name { get; set; }

  /// <summary>
  /// This controls the generated method name. 
  /// </summary>
  public string? GeneratedMethodName { get; set; } = null;
}
