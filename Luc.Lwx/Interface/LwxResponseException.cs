using System.Diagnostics.CodeAnalysis;

namespace Luc.Lwx.Interface;

public class LwxResponseException
(
    int statusCode,
    string errorMessage,
    object? errorDetails = null        
) : Exception(errorMessage) 
{    
    public int StatusCode { get; } = statusCode;
    public object? ErrorDetails { get; } = errorDetails;
}


