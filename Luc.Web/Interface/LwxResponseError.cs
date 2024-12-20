using System.Diagnostics.CodeAnalysis;

namespace Luc.Web.Interface;

[SuppressMessage("","S3376",Justification="This is not an actual error")]
public class LwxResponseError
(
    int statusCode,
    string errorMessage,
    object? errorDetails = null        
) : Exception(errorMessage)
{    
    public int StatusCode { get; } = statusCode;
    public object? ErrorDetails { get; } = errorDetails;
}


