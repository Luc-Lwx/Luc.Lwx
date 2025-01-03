using System.Diagnostics.CodeAnalysis;

namespace Luc.Lwx.Interface;


public class LwxOverrideResponseException
(
    object content,
    string? contentType = null
) : Exception("OK")
{
    public int StatusCode { get; } = 200;
    public object? Content { get; } = content;
    public string? ContentType { get; } = contentType;
}


