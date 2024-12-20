using System.Diagnostics.CodeAnalysis;

namespace Luc.Web.Interface;

[SuppressMessage("","S3376",Justification="This is not an actual error")]
public class LwxResponseSuccess
(
    object? content = null        
) : Exception("OK")
{
    public int StatusCode { get; } = 200;
    public object? Content { get; } = content;
}


